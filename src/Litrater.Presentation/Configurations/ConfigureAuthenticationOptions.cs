using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Litrater.Presentation.Configurations;

internal sealed class ConfigureAuthenticationOptions(IConfiguration configuration, IHostEnvironment environment)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(string? name, JwtBearerOptions options)
    {
        if (name != JwtBearerDefaults.AuthenticationScheme)
        {
            return;
        }

        options.Authority = configuration["Keycloak:Authority"]
                            ?? throw new InvalidOperationException("Keycloak:Authority configuration is missing.");
        options.Audience = configuration["Keycloak:Audience"]
                           ?? throw new InvalidOperationException("Keycloak:Audience configuration is missing.");
        options.RequireHttpsMetadata = configuration.GetValue("Keycloak:RequireHttpsMetadata", true);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "preferred_username",
            ValidateIssuer = environment.IsProduction()
        };
    }

    public void Configure(JwtBearerOptions options)
    {
        Configure(JwtBearerDefaults.AuthenticationScheme, options);
    }
}