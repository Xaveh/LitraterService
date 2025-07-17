using System.Security.Claims;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Users;

namespace Litrater.Presentation.Middlewares;

internal sealed class UserSyncMiddleware(
    RequestDelegate next,
    IServiceProvider serviceProvider,
    ILogger<UserSyncMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var keycloakUserIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(keycloakUserIdClaim) || !Guid.TryParse(keycloakUserIdClaim, out var keycloakUserId))
            {
                logger.LogWarning("Invalid or missing Keycloak user ID claim: {KeycloakUserIdClaim}", keycloakUserIdClaim);
                await next(context);
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var existingUser = await userRepository.GetByKeycloakUserIdAsync(keycloakUserId);
            if (existingUser is not null)
            {
                logger.LogInformation("Found existing user {UserId} for Keycloak user {KeycloakUserId}", existingUser.Id, keycloakUserId);
                await next(context);
                return;
            }

            var newUser = new User(Guid.NewGuid(), keycloakUserId);
            await userRepository.AddAsync(newUser);
            await unitOfWork.SaveChangesAsync();

            logger.LogInformation("Created new user {UserId} from Keycloak user {KeycloakUserId}", newUser.Id, keycloakUserId);
        }

        await next(context);
    }
}