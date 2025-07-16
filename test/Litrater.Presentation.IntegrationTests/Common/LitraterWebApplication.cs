using System.Text;
using Litrater.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Litrater.Presentation.IntegrationTests.Common;

public class LitraterWebApplication : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IServiceScope _scope;

    public LitraterWebApplication(string connectionString)
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:Database", connectionString);
                builder.UseSetting("Keycloak:Authority", "http://keycloak:8080/realms/litrater");
                builder.UseSetting("Keycloak:Audience", "litrater-web-api");
                builder.UseSetting("Keycloak:RequireHttpsMetadata", "false");
                builder.UseSetting("Keycloak:ValidateIssuer", "false");
                builder.UseSetting("Keycloak:ValidateAudience", "false");
                builder.UseSetting("Keycloak:ValidateLifetime", "false");
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    // Override JWT Bearer configuration for testing
                    services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
#pragma warning disable CA5404 // Disable token validation for integration tests
                            ValidateIssuer = false,
                            ValidateAudience = false,
                            ValidateLifetime = false,
#pragma warning restore CA5404
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-secret-key-for-integration-tests-must-be-at-least-32-characters-long")),
                            ClockSkew = TimeSpan.Zero
                        };
                    });
                });
            });

        HttpClient = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<LitraterDbContext>();
    }

    public HttpClient HttpClient { get; }
    public LitraterDbContext DbContext { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        HttpClient.Dispose();
        DbContext.Dispose();
        _scope.Dispose();
        _factory.Dispose();
    }
}