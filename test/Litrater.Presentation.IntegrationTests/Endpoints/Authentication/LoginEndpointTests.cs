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
        var response = await HttpClient.PostAsJsonAsync("api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var token = await response.Content.ReadAsStringAsync();
        token.ShouldNotBeNullOrWhiteSpace();
        token.Trim('"').Length.ShouldBeGreaterThan(50);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "admin@litrater.com",
            Password = "wrongpassword"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldAuthenticateAgainstDatabase()
    {
        // Arrange
        var loginRequest = new
        {
            Email = "admin@litrater.com",
            Password = "admin123"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var token = await response.Content.ReadAsStringAsync();
        token.ShouldNotBeNullOrWhiteSpace();
        token.Trim('"').Length.ShouldBeGreaterThan(50);

        // Verify token can be used for authenticated requests
        SetAuthorizationHeader(token.Trim('"'));
        var protectedResponse = await HttpClient.PostAsJsonAsync("api/v1/books", new { Title = "Test", Isbn = "123", AuthorIds = new[] { Guid.NewGuid() } });
        protectedResponse.StatusCode.ShouldNotBe(HttpStatusCode.Unauthorized);
    }
}