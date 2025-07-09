using System.Net;
using Ardalis.Result;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.Books;

public class GetBooksEndpointTests(DatabaseFixture databaseFixture) : BaseIntegrationTest(databaseFixture)
{
    [Fact]
    public async Task GetBooks_WithDefaultPagination_ShouldReturnPagedResult()
    {
        // Act
        var response = await WebApplication.HttpClient.GetAsync("api/v1/books");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.PagedInfo.PageNumber.ShouldBe(1);
        pagedResult.PagedInfo.PageSize.ShouldBe(10);
        pagedResult.Value.Count().ShouldBeGreaterThan(0);
        pagedResult.PagedInfo.TotalRecords.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetBooks_WithCustomPageSize_ShouldReturnCorrectPageSize()
    {
        // Act
        var response = await WebApplication.HttpClient.GetAsync("api/v1/books?pageSize=2");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.PagedInfo.PageNumber.ShouldBe(1);
        pagedResult.PagedInfo.PageSize.ShouldBe(2);
        pagedResult.Value.Count().ShouldBeLessThanOrEqualTo(2);

        var totalBooks = await WebApplication.DbContext.Books.CountAsync();
        pagedResult.PagedInfo.TotalPages.ShouldBe((int)Math.Ceiling((double)totalBooks / 2));
    }

    [Fact]
    public async Task GetBooks_WithSecondPage_ShouldReturnCorrectPaginationInfo()
    {
        // Act
        var response = await WebApplication.HttpClient.GetAsync("api/v1/books?page=2&pageSize=1");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.PagedInfo.PageNumber.ShouldBe(2);
        pagedResult.PagedInfo.PageSize.ShouldBe(1);
        var totalBooks = await WebApplication.DbContext.Books.CountAsync();
        pagedResult.PagedInfo.TotalRecords.ShouldBe(totalBooks);
    }

    [Fact]
    public async Task GetBooks_WithInvalidPage_ShouldReturnBadRequest()
    {
        // Act
        var response = await WebApplication.HttpClient.GetAsync("api/v1/books?page=0");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBooks_ShouldIncludeBookDetails()
    {
        // Act
        var response = await WebApplication.HttpClient.GetAsync("api/v1/books");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookDto>>>(response);
        pagedResult.ShouldNotBeNull();
        pagedResult.Value.ShouldNotBeEmpty();

        var firstBook = pagedResult.Value.First();
        firstBook.Id.ShouldNotBe(Guid.Empty);
        firstBook.Title.ShouldNotBeNullOrEmpty();
        firstBook.Isbn.ShouldNotBeNullOrEmpty();
        firstBook.AuthorIds.ShouldNotBeEmpty();
        firstBook.ReviewIds.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetBooks_ShouldReturnBooksOrderedByTitle()
    {
        // Act
        var response = await WebApplication.HttpClient.GetAsync("api/v1/books");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookDto>>>(response);
        pagedResult.ShouldNotBeNull();

        var titles = pagedResult.Value.Select(b => b.Title).ToList();
        var sortedTitles = titles.OrderBy(t => t).ToList();
        titles.ShouldBe(sortedTitles);
    }
}