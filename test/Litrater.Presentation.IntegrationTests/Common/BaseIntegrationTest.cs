using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Litrater.Presentation.IntegrationTests.Common;

public abstract class BaseIntegrationTest(DatabaseFixture databaseFixture) : IClassFixture<DatabaseFixture>, IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    protected readonly LitraterWebApplication WebApplication = new(databaseFixture.DbContainer.GetConnectionString());

    public Task InitializeAsync()
    {
        return DatabaseSeeder.SeedTestDataAsync(WebApplication.DbContext);
    }

    public Task DisposeAsync()
    {
        WebApplication.Dispose();
        return databaseFixture.ResetDatabaseAsync();
    }

    protected static async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<T>(content, JsonOptions) ?? throw new InvalidOperationException("Deserialization failed.");
    }

    protected Task LoginAsAdminAsync()
    {
        return LoginAsync("admin@litrater.com", "admin123");
    }

    protected Task LoginAsRegularUserAsync()
    {
        return LoginAsync("user@litrater.com", "user123");
    }

    private async Task LoginAsync(string email, string password)
    {
        var loginRequest = new
        {
            Email = email,
            Password = password
        };

        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadAsStringAsync();
        WebApplication.HttpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.Trim('"'));
    }
}