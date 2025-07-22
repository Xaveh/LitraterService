using System.Net;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Authors;

public class DeleteAuthorEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task DeleteAuthor_WithValidIdAndAuthorization_ShouldDeleteAuthor()
    {
        // Arrange
        LoginAsAdminAsync();

        var authorId = TestDataGenerator.Authors.Herbert.Id;

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/authors/{authorId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var deletedAuthor = await WebApplication.DbContext.Authors
            .FirstOrDefaultAsync(a => a.Id == authorId);

        deletedAuthor.ShouldBeNull();
    }


    [Fact]
    public async Task DeleteAuthor_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var authorId = TestDataGenerator.Authors.Tolkien.Id;

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/authors/{authorId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteAuthor_WithRegularUserRole_ShouldReturnForbidden()
    {
        // Arrange
        LoginAsRegularUserAsync();

        var authorId = TestDataGenerator.Authors.Rowling.Id;

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/authors/{authorId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}