using System.Net.Http.Json;
using System.Text.Json;
using Litrater.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Litrater.Presentation.IntegrationTests.Common;

public abstract class BaseIntegrationTest : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    protected readonly HttpClient HttpClient;
    protected readonly LitraterDbContext DbContext;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly IServiceScope _scope;

    protected BaseIntegrationTest(DatabaseFixture fixture)
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("ConnectionStrings:Database", fixture.DbContainer.GetConnectionString());
                builder.UseSetting("Jwt:SecretKey", "test_secret_key_123456789012345678901234567890");
                builder.UseSetting("Jwt:Issuer", "test_issuer");
                builder.UseSetting("Jwt:Audience", "test_audience");
                builder.UseEnvironment("Testing");
            });


        HttpClient = _factory.CreateClient();
        _scope = _factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<LitraterDbContext>();
    }

    protected async Task LoginAsAdminAsync()
    {
        var loginRequest = new
        {
            Email = "admin@litrater.com",
            Password = "admin123"
        };

        var response = await HttpClient.PostAsJsonAsync("api/v1/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadAsStringAsync();

        SetAuthorizationHeader(token.Trim('"'));
    }

    protected void SetAuthorizationHeader(string token)
    {
        HttpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }


    public async Task InitializeAsync()
    {
        await DbContext.Database.MigrateAsync();
        await DatabaseSeeder.SeedTestDataAsync(_scope);
    }

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();
        await DbContext.DisposeAsync();
        _scope.Dispose();
        await _factory.DisposeAsync();
    }
}