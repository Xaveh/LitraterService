using System.Net.Http.Json;

namespace Litrater.Presentation.IntegrationTests.Common;

public abstract class BaseIntegrationTest(DatabaseFixture databaseFixture) : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    protected readonly LitraterWebApplication WebApplication = new(databaseFixture.DbContainer.GetConnectionString());

    protected async Task LoginAsAdminAsync()
    {
        var loginRequest = new
        {
            Email = "admin@litrater.com",
            Password = "admin123"
        };

        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadAsStringAsync();

        SetAuthorizationHeader(token.Trim('"'));
    }

    protected void SetAuthorizationHeader(string token)
    {
        WebApplication.HttpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    public async Task InitializeAsync()
    {
        await WebApplication.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        WebApplication.Dispose();
        return Task.CompletedTask;
    }
}