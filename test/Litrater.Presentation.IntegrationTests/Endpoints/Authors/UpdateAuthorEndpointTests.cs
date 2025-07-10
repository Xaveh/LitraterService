using System.Net;
using System.Net.Http.Json;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Authors;

public class UpdateAuthorEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task UpdateAuthor_WithValidDataAndAuthorization_ShouldUpdateAuthor()
    {
        // Arrange
        await LoginAsAdminAsync();

        var authorId = TestDataGenerator.Authors.Tolkien.Id;
        var hobbitBookId = TestDataGenerator.Books.TheHobbit.Id;

        var updateAuthorRequest = new
        {
            FirstName = "John Ronald Reuel",
            LastName = "Tolkien",
            BookIds = new[] { hobbitBookId }
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/authors/{authorId}", updateAuthorRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var authorDto = await DeserializeResponse<AuthorDto>(response);

        authorDto.ShouldNotBeNull();
        authorDto.Id.ShouldBe(authorId);
        authorDto.FirstName.ShouldBe(updateAuthorRequest.FirstName);
        authorDto.LastName.ShouldBe(updateAuthorRequest.LastName);
        authorDto.BookIds.ShouldHaveSingleItem();
        authorDto.BookIds.First().ShouldBe(hobbitBookId);

        // Refresh the DbContext to get the latest data
        WebApplication.DbContext.ChangeTracker.Clear();

        var updatedAuthor = await WebApplication.DbContext.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == authorId);

        updatedAuthor.ShouldNotBeNull();
        updatedAuthor.FirstName.ShouldBe(updateAuthorRequest.FirstName);
        updatedAuthor.LastName.ShouldBe(updateAuthorRequest.LastName);
        updatedAuthor.Books.ShouldHaveSingleItem();
        updatedAuthor.Books.First().Id.ShouldBe(hobbitBookId);
        updatedAuthor.ModifiedDate.ShouldNotBeNull();
        updatedAuthor.ModifiedDate.Value.ShouldBeGreaterThan(updatedAuthor.CreatedDate);
    }

    [Fact]
    public async Task UpdateAuthor_WithMultipleBooks_ShouldUpdateAuthorWithAllBooks()
    {
        // Arrange
        await LoginAsAdminAsync();

        var authorId = TestDataGenerator.Authors.Herbert.Id;
        var hobbitBookId = TestDataGenerator.Books.TheHobbit.Id;
        var duneBookId = TestDataGenerator.Books.Dune.Id;

        var updateAuthorRequest = new
        {
            FirstName = "Frank",
            LastName = "Herbert",
            BookIds = new[] { hobbitBookId, duneBookId }
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/authors/{authorId}", updateAuthorRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var authorDto = await DeserializeResponse<AuthorDto>(response);

        authorDto.ShouldNotBeNull();
        authorDto.Id.ShouldBe(authorId);
        authorDto.FirstName.ShouldBe(updateAuthorRequest.FirstName);
        authorDto.LastName.ShouldBe(updateAuthorRequest.LastName);
        authorDto.BookIds.ShouldBe(updateAuthorRequest.BookIds, ignoreOrder: true);
    }

    [Fact]
    public async Task UpdateAuthor_WithInvalidBookIds_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsAdminAsync();

        var authorId = TestDataGenerator.Authors.Tolkien.Id;
        var updateAuthorRequest = new
        {
            FirstName = "J.R.R.",
            LastName = "Tolkien",
            BookIds = new[] { Guid.NewGuid() } // Non-existent book ID
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/authors/{authorId}", updateAuthorRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateAuthor_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var authorId = TestDataGenerator.Authors.Tolkien.Id;
        var updateAuthorRequest = new
        {
            FirstName = "J.R.R.",
            LastName = "Tolkien",
            BookIds = new[] { TestDataGenerator.Books.TheHobbit.Id }
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/authors/{authorId}", updateAuthorRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateAuthor_WithRegularUserRole_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var authorId = TestDataGenerator.Authors.Tolkien.Id;
        var updateAuthorRequest = new
        {
            FirstName = "J.R.R.",
            LastName = "Tolkien",
            BookIds = new[] { TestDataGenerator.Books.TheHobbit.Id }
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/authors/{authorId}", updateAuthorRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}