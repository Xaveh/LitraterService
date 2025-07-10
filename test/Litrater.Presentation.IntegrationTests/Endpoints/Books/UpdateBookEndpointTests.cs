using System.Net;
using System.Net.Http.Json;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Books;

public class UpdateBookEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task UpdateBook_WithValidDataAndAuthorization_ShouldUpdateBook()
    {
        // Arrange
        await LoginAsAdminAsync();

        var bookId = TestDataGenerator.Books.TheHobbit.Id;
        var tolkienAuthorId = TestDataGenerator.Authors.Tolkien.Id;

        var updateBookRequest = new
        {
            Title = "The Hobbit - Updated Edition",
            Isbn = "9780547928211",
            AuthorIds = new[] { tolkienAuthorId }
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/books/{bookId}", updateBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var bookDto = await DeserializeResponse<BookDto>(response);

        bookDto.ShouldNotBeNull();
        bookDto.Id.ShouldBe(bookId);
        bookDto.Title.ShouldBe(updateBookRequest.Title);
        bookDto.Isbn.ShouldBe(updateBookRequest.Isbn);
        bookDto.AuthorIds.ShouldHaveSingleItem();
        bookDto.AuthorIds.First().ShouldBe(tolkienAuthorId);

        // Refresh the DbContext to get the latest data
        WebApplication.DbContext.ChangeTracker.Clear();

        var updatedBook = await WebApplication.DbContext.Books
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.Id == bookId);

        updatedBook.ShouldNotBeNull();
        updatedBook.Title.ShouldBe(updateBookRequest.Title);
        updatedBook.Isbn.ShouldBe(updateBookRequest.Isbn);
        updatedBook.Authors.ShouldHaveSingleItem();
        updatedBook.Authors.First().Id.ShouldBe(tolkienAuthorId);
    }

    [Fact]
    public async Task UpdateBook_WithNonExistentBook_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsAdminAsync();

        var nonExistentId = Guid.NewGuid();
        var updateBookRequest = new
        {
            Title = "Non-existent Book",
            Isbn = "9780123456789",
            AuthorIds = new[] { TestDataGenerator.Authors.Tolkien.Id }
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/books/{nonExistentId}", updateBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateBook_WithInvalidAuthorIds_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsAdminAsync();

        var bookId = TestDataGenerator.Books.TheHobbit.Id;
        var updateBookRequest = new
        {
            Title = "The Hobbit - Updated Edition",
            Isbn = "9780547928211",
            AuthorIds = new[] { Guid.NewGuid() } // Non-existent author ID
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/books/{bookId}", updateBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateBook_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var bookId = TestDataGenerator.Books.TheHobbit.Id;
        var updateBookRequest = new
        {
            Title = "The Hobbit - Updated Edition",
            Isbn = "9780547928211",
            AuthorIds = new[] { TestDataGenerator.Authors.Tolkien.Id }
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/books/{bookId}", updateBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateBook_WithRegularUserRole_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var bookId = TestDataGenerator.Books.TheHobbit.Id;
        var updateBookRequest = new
        {
            Title = "The Hobbit - Updated Edition",
            Isbn = "9780547928211",
            AuthorIds = new[] { TestDataGenerator.Authors.Tolkien.Id }
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/books/{bookId}", updateBookRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}