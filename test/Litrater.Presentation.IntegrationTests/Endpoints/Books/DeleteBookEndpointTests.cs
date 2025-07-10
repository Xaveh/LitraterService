using System.Net;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Books;

public class DeleteBookEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task DeleteBook_WithValidIdAndAuthorization_ShouldDeleteBook()
    {
        // Arrange
        await LoginAsAdminAsync();

        var bookId = TestDataGenerator.Books.TheHobbit.Id;

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/books/{bookId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var deletedBook = await WebApplication.DbContext.Books
            .FirstOrDefaultAsync(b => b.Id == bookId);

        deletedBook.ShouldBeNull();
    }



    [Fact]
    public async Task DeleteBook_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var bookId = TestDataGenerator.Books.TheHobbit.Id;

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/books/{bookId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteBook_WithRegularUserRole_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var bookId = TestDataGenerator.Books.TheHobbit.Id;

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/books/{bookId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}