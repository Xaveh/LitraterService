using System.Net;
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

        var bookDto = await DeserializeResponse<BookDto>(response);

        bookDto.ShouldNotBeNull();
        bookDto.Id.ShouldBe(hobbitBook.Id);
        bookDto.Title.ShouldBe(hobbitBook.Title);
        bookDto.Isbn.ShouldBe(hobbitBook.Isbn);
        bookDto.AuthorIds.Count().ShouldBe(hobbitBook.Authors.Count);
        bookDto.AuthorIds.ShouldContain(hobbitBook.Authors.First().Id);
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

        var bookDto = await DeserializeResponse<BookDto>(response);

        bookDto.ShouldNotBeNull();
    }
}