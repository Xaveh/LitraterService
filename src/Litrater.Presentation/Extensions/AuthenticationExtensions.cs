using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Litrater.Presentation.Extensions;

internal static class AuthenticationExtensions
{
    internal static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var authority = configuration["Keycloak:Authority"]
                                ?? throw new InvalidOperationException("Keycloak:Authority configuration is missing.");
                var audience = configuration["Keycloak:Audience"]
                               ?? throw new InvalidOperationException("Keycloak:Audience configuration is missing.");

                options.Authority = authority;
                options.Audience = audience;
                options.RequireHttpsMetadata = configuration.GetValue("Keycloak:RequireHttpsMetadata", true);

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = configuration.GetValue("Keycloak:ValidateIssuer", true),
                    ValidateAudience = configuration.GetValue("Keycloak:ValidateAudience", true),
                    ValidateLifetime = configuration.GetValue("Keycloak:ValidateLifetime", true),
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = authority,
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.FromMinutes(configuration.GetValue("Keycloak:ClockSkewMinutes", 5)),
                    RoleClaimType = configuration.GetValue<string>("Keycloak:RoleClaimType") ?? "realm_access.roles",
                    NameClaimType = configuration.GetValue<string>("Keycloak:NameClaimType") ?? "preferred_username"
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is not ClaimsIdentity claimsIdentity)
                        {
                            return Task.CompletedTask;
                        }

                        var resourceAccessClaim = claimsIdentity.FindFirst("resource_access");
                        if (resourceAccessClaim is null)
                        {
                            return Task.CompletedTask;
                        }

                        var resourceAccess = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(resourceAccessClaim.Value);
                        if (resourceAccess is null || !resourceAccess.TryGetValue("litrater-web-api", out var clientValue))
                        {
                            return Task.CompletedTask;
                        }

                        var clientElement = (System.Text.Json.JsonElement)clientValue;
                        if (!clientElement.TryGetProperty("roles", out var rolesElement) || rolesElement.ValueKind != System.Text.Json.JsonValueKind.Array)
                        {
                            return Task.CompletedTask;
                        }

                        foreach (var roleValue in rolesElement.EnumerateArray().Select(role => role.GetString()).OfType<string>())
                        {
                            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                        }

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }
}