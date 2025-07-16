using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Litrater.Presentation.IntegrationTests.Common;

internal static class TestJwtTokenGenerator
{
    private const string SecretKey = "test-secret-key-for-integration-tests-must-be-at-least-32-characters-long";
    private const string Issuer = "http://keycloak:8080/realms/litrater";
    private const string Audience = "litrater-web-api";

    private static string GenerateToken(string userId, string email, string firstName, string lastName, string[] roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.GivenName, firstName),
            new(ClaimTypes.Surname, lastName),
            new("preferred_username", email),
            new("resource_access", $$$"""{"litrater-web-api": {"roles": [{{{string.Join(",", roles.Select(r => $"\"{r}\""))}}}]}}""")
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateAdminToken()
    {
        return GenerateToken(
            TestDataGenerator.Users.Admin.Id.ToString(),
            "admin@litrater.com",
            "Admin",
            "User",
            ["admin", "user"]);
    }

    public static string GenerateUserToken()
    {
        return GenerateToken(
            TestDataGenerator.Users.Regular.Id.ToString(),
            "user@litrater.com",
            "Regular",
            "User",
            ["user"]);
    }
}