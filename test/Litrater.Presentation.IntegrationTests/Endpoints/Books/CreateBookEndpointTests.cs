using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Books;

public class CreateBookEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task CreateBook_WithValidDataAndAuthorization_ShouldCreateBook()
    {
        // Arrange
        await LoginAsAdminAsync();

        var tolkienAuthorId = TestDataGenerator.Authors.Tolkien.Id;

        var createBookRequest = new
        {
            Title = "The Fellowship of the Ring",
            Isbn = "9780547928211",
            AuthorIds = new[] { tolkienAuthorId }
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/books", createBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var content = await response.Content.ReadAsStringAsync();
        var bookDto = JsonSerializer.Deserialize<BookDto>(content);

        bookDto.ShouldNotBeNull();
        bookDto.Title.ShouldBe(createBookRequest.Title);
        bookDto.Isbn.ShouldBe(createBookRequest.Isbn);
        bookDto.AuthorIds.ShouldHaveSingleItem();
        bookDto.AuthorIds.First().ShouldBe(tolkienAuthorId);
    }

    [Fact]
    public async Task CreateBook_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var createBookRequest = new
        {
            Title = "Test Book",
            Isbn = "9780123456789",
            AuthorIds = new[] { Guid.NewGuid() }
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/books", createBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBook_WithRegularUserRole_ShouldReturnForbidden()
    {
        // Arrange
        var token = await LoginAsRegularUserAsync();
        SetAuthorizationHeader(token);

        var createBookRequest = new
        {
            Title = "Test Book",
            Isbn = "9780123456789",
            AuthorIds = new[] { Guid.NewGuid() }
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/books", createBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateBook_WithValidData_ShouldPersistBookInDatabase()
    {
        // Arrange
        await LoginAsAdminAsync();

        var tolkienAuthorId = TestDataGenerator.Authors.Tolkien.Id;

        var createBookRequest = new
        {
            Title = "The Two Towers",
            Isbn = "9780547928226",
            AuthorIds = new[] { tolkienAuthorId }
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync("api/v1/books", createBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var bookDto = JsonSerializer.Deserialize<BookDto>(content);
        bookDto.ShouldNotBeNull();

        // Verify book is persisted in database
        var persistedBook = await DbContext.Books
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.Id == bookDto.Id);

        persistedBook.ShouldNotBeNull();
        persistedBook.Title.ShouldBe(createBookRequest.Title);
        persistedBook.Isbn.ShouldBe(createBookRequest.Isbn);
        persistedBook.Authors.ShouldHaveSingleItem();
        persistedBook.Authors.First().Id.ShouldBe(tolkienAuthorId);
    }

    private async Task<string> LoginAsRegularUserAsync()
    {
        var loginRequest = new
        {
            Email = "user@litrater.com",
            Password = "user123"
        };
        var response = await HttpClient.PostAsJsonAsync("api/v1/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        var token = await response.Content.ReadAsStringAsync();
        return token.Trim('"');
    }
}