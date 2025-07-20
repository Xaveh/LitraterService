using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Litrater.Presentation.Extensions;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["Keycloak:Authority"]
                                    ?? throw new InvalidOperationException("Keycloak:Authority configuration is missing.");
                options.Audience = configuration["Keycloak:Audience"]
                                   ?? throw new InvalidOperationException("Keycloak:Audience configuration is missing.");
                options.RequireHttpsMetadata = configuration.GetValue("Keycloak:RequireHttpsMetadata", true);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "preferred_username",
                    ValidateIssuer = isProduction
                };
            });

        return services;
    }
}