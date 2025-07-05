using System.Net;
using Litrater.Presentation.IntegrationTests.Common;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.HealthCheck;

public class HealthCheckEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task HealthCheck_WithoutAuthentication_ShouldReturnHealthyStatus()
    {
        // Act
        var response = await WebApplication.HttpClient.GetAsync("health");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Healthy");
    }
}