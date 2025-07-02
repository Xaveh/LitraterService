using System.Net;
using System.Net.Http.Json;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Litrater.Application.Features.Authentication.Dtos;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Authentication;

public class RegisterEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task Register_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var registerRequest = new
        {
            Email = "newuser@example.com",
            Password = "NewUser123!",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/auth/register", registerRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
        userDto.ShouldNotBeNull();
        userDto.Email.ShouldBe(registerRequest.Email);
        userDto.FirstName.ShouldBe(registerRequest.FirstName);
        userDto.LastName.ShouldBe(registerRequest.LastName);

        var persistedUser = await WebApplication.DbContext.Users
            .FirstOrDefaultAsync(u => u.Email == registerRequest.Email);

        persistedUser.ShouldNotBeNull();
        persistedUser.Email.ShouldBe(registerRequest.Email);
        persistedUser.FirstName.ShouldBe(registerRequest.FirstName);
        persistedUser.LastName.ShouldBe(registerRequest.LastName);
        persistedUser.PasswordHash.ShouldNotBeNullOrWhiteSpace();
        persistedUser.PasswordHash.ShouldNotBe(registerRequest.Password); // Should be hashed
    }

    [Fact]
    public async Task Register_WithExistingEmail_ShouldReturnConflict()
    {
        // Arrange
        var registerRequest = new
        {
            Email = "admin@litrater.com",
            Password = "NewUser123!",
            FirstName = "New",
            LastName = "User"
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/auth/register", registerRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }
}