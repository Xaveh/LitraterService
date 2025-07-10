using System.Net;
using Ardalis.Result;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.BookReviews;

public class GetBookReviewsByBookIdEndpointTests(DatabaseFixture databaseFixture) : BaseIntegrationTest(databaseFixture)
{
    [Fact]
    public async Task GetBookReviewsByBookId_WithDefaultPagination_ShouldReturnPagedResult()
    {
        // Arrange
        var bookId = TestDataGenerator.Books.TheHobbit.Id;

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{bookId}/reviews");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.PagedInfo.PageNumber.ShouldBe(1);
        pagedResult.PagedInfo.PageSize.ShouldBe(10);
        pagedResult.Value.Count().ShouldBe(2); // TheHobbit has 2 reviews
        pagedResult.PagedInfo.TotalRecords.ShouldBe(2);
        pagedResult.PagedInfo.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task GetBookReviewsByBookId_WithCustomPageSize_ShouldReturnCorrectPageSize()
    {
        // Arrange
        var bookId = TestDataGenerator.Books.Dune.Id; // Dune has 2 reviews

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{bookId}/reviews?pageSize=1");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.PagedInfo.PageNumber.ShouldBe(1);
        pagedResult.PagedInfo.PageSize.ShouldBe(1);
        pagedResult.Value.Count().ShouldBe(1);
        pagedResult.PagedInfo.TotalRecords.ShouldBe(2);
        pagedResult.PagedInfo.TotalPages.ShouldBe(2);
    }

    [Fact]
    public async Task GetBookReviewsByBookId_WithSecondPage_ShouldReturnCorrectPaginationInfo()
    {
        // Arrange
        var bookId = TestDataGenerator.Books.Dune.Id; // Dune has 2 reviews

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{bookId}/reviews?page=2&pageSize=1");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.PagedInfo.PageNumber.ShouldBe(2);
        pagedResult.PagedInfo.PageSize.ShouldBe(1);
        pagedResult.Value.Count().ShouldBe(1);
        pagedResult.PagedInfo.TotalRecords.ShouldBe(2);
        pagedResult.PagedInfo.TotalPages.ShouldBe(2);
    }

    [Fact]
    public async Task GetBookReviewsByBookId_WithInvalidPage_ShouldReturnBadRequest()
    {
        // Arrange
        var bookId = TestDataGenerator.Books.TheHobbit.Id;

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{bookId}/reviews?page=0");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBookReviewsByBookId_WithInvalidPageSize_ShouldReturnBadRequest()
    {
        // Arrange
        var bookId = TestDataGenerator.Books.TheHobbit.Id;

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{bookId}/reviews?pageSize=0");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetBookReviewsByBookId_WithNonExistentBook_ShouldReturnEmptyResult()
    {
        // Arrange
        var nonExistentBookId = Guid.NewGuid();

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{nonExistentBookId}/reviews");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.Value.Count().ShouldBe(0);
        pagedResult.PagedInfo.TotalRecords.ShouldBe(0);
        pagedResult.PagedInfo.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task GetBookReviewsByBookId_ShouldIncludeReviewDetails()
    {
        // Arrange
        var bookId = TestDataGenerator.Books.TheHobbit.Id;

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{bookId}/reviews");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);
        pagedResult.ShouldNotBeNull();
        pagedResult.Value.ShouldNotBeEmpty();

        var firstReview = pagedResult.Value.First();
        firstReview.Id.ShouldNotBe(Guid.Empty);
        firstReview.Content.ShouldNotBeNullOrEmpty();
        firstReview.Rating.ShouldBeInRange(1, 5);
        firstReview.BookId.ShouldBe(bookId);
        firstReview.UserId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task GetBookReviewsByBookId_ShouldReturnReviewsOrderedByCreatedDateDescending()
    {
        // Arrange
        var bookId = TestDataGenerator.Books.TheHobbit.Id;

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{bookId}/reviews");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);
        pagedResult.ShouldNotBeNull();
        pagedResult.Value.Count().ShouldBeGreaterThan(1);

        // Get the actual reviews from database to check ordering
        var reviewsFromDb = await WebApplication.DbContext.BookReviews
            .Where(br => br.BookId == bookId)
            .OrderByDescending(br => br.CreatedDate)
            .ToListAsync();

        var reviewIds = pagedResult.Value.Select(r => r.Id).ToList();
        var expectedIds = reviewsFromDb.Select(r => r.Id).ToList();

        reviewIds.ShouldBe(expectedIds);
    }

    [Fact]
    public async Task GetBookReviewsByBookId_WithBookHavingNoReviews_ShouldReturnEmptyResult()
    {
        // Arrange - HarryPotter has only 1 review, let's use a book with no reviews
        var bookId = TestDataGenerator.Books.HarryPotter.Id;

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/books/{bookId}/reviews?page=2");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.Value.Count().ShouldBe(0);
        pagedResult.PagedInfo.PageNumber.ShouldBe(2);
        pagedResult.PagedInfo.TotalRecords.ShouldBe(1); // HarryPotter has 1 review
    }
}