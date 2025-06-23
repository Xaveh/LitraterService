using System.Net;
using Litrater.Presentation.IntegrationTests.Common;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.HealthCheck;

public class HealthCheckEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task HealthCheck_ShouldReturnHealthyStatus()
    {
        // Act
        var response = await WebApplication.HttpClient.GetAsync("health");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("text/plain");

        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("Healthy");
    }

    [Fact]
    public async Task HealthCheck_ShouldBeAccessibleWithoutAuthentication()
    {
        // Act - No authentication header set
        var response = await WebApplication.HttpClient.GetAsync("health");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}