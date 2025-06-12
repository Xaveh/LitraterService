using System.Text;
using Asp.Versioning;
using Litrater.Domain.Users;
using Litrater.Infrastructure.Authentication;
using Litrater.Presentation.Authorization;
using Litrater.Presentation.Configurations;
using Litrater.Presentation.Extensions;
using Litrater.Presentation.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Litrater.Presentation;

internal static class DependencyInjection
{
    internal static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                    ?? throw new InvalidOperationException($"JWT configuration section '{JwtSettings.SectionName}' is missing.");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthorizationPolicies.AdminOnly, policy => policy.RequireRole(nameof(UserRole.Admin)))
            .AddPolicy(AuthorizationPolicies.UserOrAdmin, policy => policy.RequireRole(nameof(UserRole.User), nameof(UserRole.Admin)));

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