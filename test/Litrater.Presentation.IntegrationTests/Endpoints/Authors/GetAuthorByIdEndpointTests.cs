using System.Net;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Authors;

public class GetAuthorByIdEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task GetAuthorById_WithExistingAuthor_ShouldReturnAuthorData()
    {
        // Arrange
        var tolkienAuthor = await WebApplication.DbContext.Authors
            .Include(author => author.Books)
            .FirstAsync(a => a.FirstName == "J.R.R." && a.LastName == "Tolkien");

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/authors/{tolkienAuthor.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        response.Content.Headers.ContentType?.MediaType.ShouldBe("application/json");

        var authorDto = await DeserializeResponse<AuthorDto>(response);

        authorDto.ShouldNotBeNull();
        authorDto.Id.ShouldBe(tolkienAuthor.Id);
        authorDto.FirstName.ShouldBe(tolkienAuthor.FirstName);
        authorDto.LastName.ShouldBe(tolkienAuthor.LastName);
        authorDto.BookIds.Count().ShouldBe(tolkienAuthor.Books.Count);
        if (tolkienAuthor.Books.Count > 0)
        {
            authorDto.BookIds.ShouldContain(tolkienAuthor.Books.First().Id);
        }
    }



    [Fact]
    public async Task GetAuthorById_ShouldBeAccessibleWithoutAuthentication()
    {
        // Arrange
        var author = await WebApplication.DbContext.Authors.FirstAsync();

        // Act - No authentication header set
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/authors/{author.Id}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var authorDto = await DeserializeResponse<AuthorDto>(response);

        authorDto.ShouldNotBeNull();
    }
}