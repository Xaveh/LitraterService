using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.CQRS;
using Microsoft.Extensions.Logging;

namespace Litrater.Application.Features.Authentication.Queries.Login;

public sealed class LoginQueryHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenProvider tokenProvider,
    ILogger<LoginQueryHandler> logger) : IQueryHandler<LoginQuery, string>
{
    public async Task<Result<string>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("Login attempt for email: {Email}", query.Email);

        var user = await userRepository.GetByEmailAsync(query.Email, cancellationToken);

        if (user is null)
        {
            logger.LogWarning("Security: Login failed for email {Email} - user not found", query.Email);
            return Result.Unauthorized();
        }

        if (!passwordHasher.Verify(query.Password, user.PasswordHash))
        {
            logger.LogWarning("Security: Login failed for user {UserId} ({Email}) - invalid password", user.Id, user.Email);
            return Result.Unauthorized();
        }

        if (!user.IsActive)
        {
            logger.LogWarning("Security: Login failed for user {UserId} ({Email}) - account is inactive", user.Id, user.Email);
            return Result.Forbidden();
        }

        var token = tokenProvider.GenerateToken(user);

        logger.LogInformation("Security: Successful login for user {UserId} ({Email}) with role {UserRole}", user.Id, user.Email, user.UserRole);

        return Result<string>.Success(token);
    }
}