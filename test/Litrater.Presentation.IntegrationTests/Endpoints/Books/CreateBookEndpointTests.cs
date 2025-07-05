using System.Net;
using System.Net.Http.Json;
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
        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/books", createBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var bookDto = await DeserializeResponse<BookDto>(response);

        bookDto.ShouldNotBeNull();
        bookDto.Title.ShouldBe(createBookRequest.Title);
        bookDto.Isbn.ShouldBe(createBookRequest.Isbn);
        bookDto.AuthorIds.ShouldHaveSingleItem();
        bookDto.AuthorIds.First().ShouldBe(tolkienAuthorId);

        var persistedBook = await WebApplication.DbContext.Books
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.Id == bookDto.Id);

        persistedBook.ShouldNotBeNull();
        persistedBook.Title.ShouldBe(createBookRequest.Title);
        persistedBook.Isbn.ShouldBe(createBookRequest.Isbn);
        persistedBook.Authors.ShouldHaveSingleItem();
        persistedBook.Authors.First().Id.ShouldBe(tolkienAuthorId);
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
        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/books", createBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBook_WithRegularUserRole_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var createBookRequest = new
        {
            Title = "Test Book",
            Isbn = "9780123456789",
            AuthorIds = new[] { Guid.NewGuid() }
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync("api/v1/books", createBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}