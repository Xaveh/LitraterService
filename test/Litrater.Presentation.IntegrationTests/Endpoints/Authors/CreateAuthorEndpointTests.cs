using System.Net;
using System.Net.Http.Json;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Authors;

public class CreateAuthorEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task CreateAuthor_WithValidDataAndAuthorization_ShouldCreateAuthor()
    {
        // Arrange
        await LoginAsAdminAsync();

        var createAuthorRequest = new
        {
            FirstName = "George",
            LastName = "Orwell"
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/authors", createAuthorRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var authorDto = await DeserializeResponse<AuthorDto>(response);

        authorDto.ShouldNotBeNull();
        authorDto.FirstName.ShouldBe(createAuthorRequest.FirstName);
        authorDto.LastName.ShouldBe(createAuthorRequest.LastName);
        authorDto.Id.ShouldNotBe(Guid.Empty);
        authorDto.BookIds.ShouldBeEmpty();

        var persistedAuthor = await WebApplication.DbContext.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == authorDto.Id);

        persistedAuthor.ShouldNotBeNull();
        persistedAuthor.FirstName.ShouldBe(createAuthorRequest.FirstName);
        persistedAuthor.LastName.ShouldBe(createAuthorRequest.LastName);
        persistedAuthor.Books.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateAuthor_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var createAuthorRequest = new
        {
            FirstName = "Test",
            LastName = "Author"
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/authors", createAuthorRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateAuthor_WithRegularUserRole_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var createAuthorRequest = new
        {
            FirstName = "Test",
            LastName = "Author"
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/authors", createAuthorRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Theory]
    [InlineData("", "LastName")]
    [InlineData("FirstName", "")]
    [InlineData(null, "LastName")]
    [InlineData("FirstName", null)]
    public async Task CreateAuthor_WithInvalidData_ShouldReturnBadRequest(string? firstName, string? lastName)
    {
        // Arrange
        await LoginAsAdminAsync();

        var createAuthorRequest = new
        {
            FirstName = firstName,
            LastName = lastName
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/authors", createAuthorRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}