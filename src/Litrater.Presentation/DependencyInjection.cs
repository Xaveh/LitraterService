using Asp.Versioning;
using Litrater.Presentation.Configurations;
using Litrater.Presentation.Extensions;
using Litrater.Presentation.Middlewares;

namespace Litrater.Presentation;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddAuthorization();
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

        return services;
    }
}