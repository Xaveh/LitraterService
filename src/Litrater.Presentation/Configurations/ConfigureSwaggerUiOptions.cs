using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Litrater.Presentation.Configurations;

internal sealed class ConfigureSwaggerUiOptions(IApiVersionDescriptionProvider provider, IConfiguration configuration)
    : IConfigureOptions<SwaggerUIOptions>
{
    public void Configure(SwaggerUIOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"Litrater API {description.GroupName.ToUpperInvariant()}");
        }

        options.OAuthClientId(configuration["Keycloak:Audience"]);
    }
}