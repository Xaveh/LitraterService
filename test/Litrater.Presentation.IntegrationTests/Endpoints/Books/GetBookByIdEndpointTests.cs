using System.Net;
using System.Text.Json;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Books;

public class GetBookByIdEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task GetBookById_WithExistingBook_ShouldReturnBookData()
    {
        // Arrange
        var hobbitBook = await WebApplication.DbContext.Books.FirstAsync(b => b.Title == "The Hobbit");

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{hobbitBook.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var content = await response.Content.ReadAsStringAsync();
        var bookDto = JsonSerializer.Deserialize<BookDto>(content);

        bookDto.ShouldNotBeNull();
        bookDto.Id.ShouldBe(hobbitBook.Id);
        bookDto.Title.ShouldBe("The Hobbit");
        bookDto.Isbn.ShouldBe("9780547928227");
        bookDto.AuthorIds.ShouldHaveSingleItem();
    }

    [Fact]
    public async Task GetBookById_WithNonExistentBook_ShouldReturnNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{nonExistentId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBookById_ShouldBeAccessibleWithoutAuthentication()
    {
        // Arrange
        var book = await WebApplication.DbContext.Books.FirstAsync();

        // Act - No authentication header set
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{book.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var bookDto = JsonSerializer.Deserialize<BookDto>(content);
        bookDto.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetBookById_ShouldReturnExactDataFromDatabase()
    {
        // Arrange
        var databaseBook = await WebApplication.DbContext.Books
            .Include(b => b.Authors)
            .FirstAsync(b => b.Title == "The Hobbit");

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{databaseBook.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var returnedBookDto = JsonSerializer.Deserialize<BookDto>(content);

        returnedBookDto.ShouldNotBeNull();
        returnedBookDto.Id.ShouldBe(databaseBook.Id);
        returnedBookDto.Title.ShouldBe(databaseBook.Title);
        returnedBookDto.Isbn.ShouldBe(databaseBook.Isbn);
        returnedBookDto.AuthorIds.Count().ShouldBe(databaseBook.Authors.Count);
        returnedBookDto.AuthorIds.ShouldContain(databaseBook.Authors.First().Id);
    }
}