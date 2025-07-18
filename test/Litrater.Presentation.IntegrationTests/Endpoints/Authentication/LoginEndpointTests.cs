using System.Net;
using System.Net.Http.Json;
using Litrater.Presentation.IntegrationTests.Common;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Authentication;

public class LoginEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnJwtToken()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "admin@litrater.com",
            Password = "admin123"
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var token = await response.Content.ReadAsStringAsync();
        token.ShouldNotBeNullOrWhiteSpace();
        token.Trim('"').Length.ShouldBeGreaterThan(50);
    }


}