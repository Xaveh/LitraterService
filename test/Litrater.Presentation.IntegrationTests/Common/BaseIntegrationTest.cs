using System.Net.Http.Headers;
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

    protected void LoginAsAdminAsync()
    {
        var token = TestJwtTokenGenerator.GenerateAdminToken();
        WebApplication.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected void LoginAsRegularUserAsync()
    {
        var token = TestJwtTokenGenerator.GenerateUserToken();
        WebApplication.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}