using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.Common;
using Litrater.Domain.Users;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Litrater.Infrastructure.Authentication;

internal sealed class TokenGenerator(
    IOptions<JwtSettings> jwtOptions,
    IDateTimeProvider dateTimeProvider,
    ILogger<TokenGenerator> logger) : ITokenProvider
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;

    public string GenerateToken(User user)
    {
        logger.LogInformation("Generating JWT token for user {UserId} ({UserEmail}) with role {UserRole}", user.Id, user.Email, user.UserRole);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.UserRole.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var now = dateTimeProvider.UtcNow;
        var expiresAt = now.AddMinutes(_jwtSettings.ExpirationInMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            notBefore: now.LocalDateTime,
            expires: expiresAt.LocalDateTime,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        logger.LogInformation("Successfully generated JWT token for user {UserId}. Token expires at {ExpiresAt}", user.Id, expiresAt);

        return tokenString;
    }
}