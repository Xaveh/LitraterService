using System.Security.Claims;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Users;

namespace Litrater.Presentation.Middlewares;

internal sealed class UserSyncMiddleware(
    RequestDelegate next,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    ILogger<UserSyncMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var keycloakUserId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(keycloakUserId))
            {
                return;
            }

            var existingUser = await userRepository.GetByKeycloakUserIdAsync(keycloakUserId);
            if (existingUser is not null)
            {
                logger.LogInformation("Found existing user {UserId} for Keycloak user {KeycloakUserId}", existingUser.Id, keycloakUserId);
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