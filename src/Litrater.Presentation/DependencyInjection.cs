using Asp.Versioning;
using Keycloak.AuthServices.Authorization;
using Litrater.Presentation.Authorization;
using Litrater.Presentation.Configurations;
using Litrater.Presentation.Extensions;
using Litrater.Presentation.Middlewares;

namespace Litrater.Presentation;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration, bool isProduction)
    {
        services.AddKeycloakAuthentication(configuration, isProduction);

        services.AddAuthorization()
            .AddKeycloakAuthorization(options => options.RolesResource = configuration["Keycloak:Audience"])
            .AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.AdminOnly, policy => policy.RequireResourceRoles("admin"))
            .AddPolicy(AuthorizationPolicies.UserOrAdmin, policy => policy.RequireResourceRoles("admin", "user"));

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddEndpoints(typeof(DependencyInjection).Assembly);
        services.ConfigureOptions<ConfigureSwaggerOptions>();
        services.AddLifecycleLogging();

        return services;
    }
}