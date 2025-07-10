using Litrater.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

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
                builder.UseSetting("Jwt:SecretKey", "test_secret_key_123456789012345678901234567890");
                builder.UseSetting("Jwt:Issuer", "test_issuer");
                builder.UseSetting("Jwt:Audience", "test_audience");
                builder.UseEnvironment("Testing");
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