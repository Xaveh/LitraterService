using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Litrater.Presentation.Configurations;

internal sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{configuration["Keycloak:Authority"]}/protocol/openid-connect/auth".Replace("keycloak:", "localhost:")),
                    TokenUrl = new Uri($"{configuration["Keycloak:Authority"]}/protocol/openid-connect/token".Replace("keycloak:", "localhost:")),
                    Scopes = new Dictionary<string, string>
                    {
                        { "openid", "OpenID" },
                        { "profile", "Profile" },
                        { "email", "Email" },
                    }
                }
            },
            Description = "Keycloak OAuth2/OpenID Connect"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                },
                ["openid", "profile", "email"]
            }
        });
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var openApiInfo = new OpenApiInfo
        {
            Title = $"Litrater API v{description.ApiVersion}", Version = description.ApiVersion.ToString()
        };

        if (description.IsDeprecated)
        {
            openApiInfo.Description += " This API version has been deprecated.";
        }

        return openApiInfo;
    }
}