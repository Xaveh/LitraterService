using System.Net;
using Ardalis.Result;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.BookReviews;

public class GetBookReviewsByUserIdEndpointTests(DatabaseFixture databaseFixture) : BaseIntegrationTest(databaseFixture)
{
    [Fact]
    public async Task GetBookReviewsByUserId_WithDefaultPagination_ShouldReturnPagedResult()
    {
        // Arrange
        var userId = TestDataGenerator.Users.Regular.Id;

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/users/{userId}/reviews");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.PagedInfo.PageNumber.ShouldBe(1);
        pagedResult.PagedInfo.PageSize.ShouldBe(10);
        pagedResult.Value.Count().ShouldBe(3); // Regular user has 3 reviews
        pagedResult.PagedInfo.TotalRecords.ShouldBe(3);
        pagedResult.PagedInfo.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task GetBookReviewsByUserId_WithCustomPageSize_ShouldReturnCorrectPageSize()
    {
        // Arrange
        var userId = TestDataGenerator.Users.Regular.Id; // Regular user has 3 reviews

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/users/{userId}/reviews?pageSize=2");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.PagedInfo.PageNumber.ShouldBe(1);
        pagedResult.PagedInfo.PageSize.ShouldBe(2);
        pagedResult.Value.Count().ShouldBe(2);
        pagedResult.PagedInfo.TotalRecords.ShouldBe(3);
        pagedResult.PagedInfo.TotalPages.ShouldBe(2);
    }

    [Fact]
    public async Task GetBookReviewsByUserId_WithSecondPage_ShouldReturnCorrectPaginationInfo()
    {
        // Arrange
        var userId = TestDataGenerator.Users.Regular.Id; // Regular user has 3 reviews

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/users/{userId}/reviews?page=2&pageSize=2");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.PagedInfo.PageNumber.ShouldBe(2);
        pagedResult.PagedInfo.PageSize.ShouldBe(2);
        pagedResult.Value.Count().ShouldBe(1);
        pagedResult.PagedInfo.TotalRecords.ShouldBe(3);
        pagedResult.PagedInfo.TotalPages.ShouldBe(2);
    }



    [Fact]
    public async Task GetBookReviewsByUserId_WithNonExistentUser_ShouldReturnEmptyResult()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/users/{nonExistentUserId}/reviews");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.Value.Count().ShouldBe(0);
        pagedResult.PagedInfo.TotalRecords.ShouldBe(0);
        pagedResult.PagedInfo.TotalPages.ShouldBe(0);
    }

    [Fact]
    public async Task GetBookReviewsByUserId_ShouldIncludeReviewDetails()
    {
        // Arrange
        var userId = TestDataGenerator.Users.Regular.Id;

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/users/{userId}/reviews");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);
        pagedResult.ShouldNotBeNull();
        pagedResult.Value.ShouldNotBeEmpty();

        var firstReview = pagedResult.Value.First();
        firstReview.Id.ShouldNotBe(Guid.Empty);
        firstReview.Content.ShouldNotBeNullOrEmpty();
        firstReview.Rating.ShouldBeInRange(1, 5);
        firstReview.BookId.ShouldNotBe(Guid.Empty);
        firstReview.UserId.ShouldBe(userId);
    }

    [Fact]
    public async Task GetBookReviewsByUserId_ShouldReturnReviewsOrderedByCreatedDateDescending()
    {
        // Arrange
        var userId = TestDataGenerator.Users.Regular.Id;

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/users/{userId}/reviews");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);
        pagedResult.ShouldNotBeNull();
        pagedResult.Value.Count().ShouldBeGreaterThan(1);

        // Get the actual reviews from database to check ordering
        var reviewsFromDb = await WebApplication.DbContext.BookReviews
            .Where(br => br.UserId == userId)
            .OrderByDescending(br => br.CreatedDate)
            .ToListAsync();

        var reviewIds = pagedResult.Value.Select(r => r.Id).ToList();
        var expectedIds = reviewsFromDb.Select(r => r.Id).ToList();

        reviewIds.ShouldBe(expectedIds);
    }

    [Fact]
    public async Task GetBookReviewsByUserId_WithPageExceedingTotalPages_ShouldReturnEmptyResult()
    {
        // Arrange
        var userId = TestDataGenerator.Users.Regular.Id; // Regular user has 3 reviews

        // Act
        var response = await WebApplication.HttpClient.GetAsync($"api/v1/users/{userId}/reviews?page=5&pageSize=10");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var pagedResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(response);

        pagedResult.ShouldNotBeNull();
        pagedResult.Value.Count().ShouldBe(0);
        pagedResult.PagedInfo.PageNumber.ShouldBe(5);
        pagedResult.PagedInfo.TotalRecords.ShouldBe(3);
        pagedResult.PagedInfo.TotalPages.ShouldBe(1);
    }

    [Fact]
    public async Task GetBookReviewsByUserId_ShouldReturnOnlyUserSpecificReviews()
    {
        // Arrange
        var regularUserId = TestDataGenerator.Users.Regular.Id;
        var adminUserId = TestDataGenerator.Users.Admin.Id;

        // Act
        var regularUserResponse = await WebApplication.HttpClient.GetAsync($"api/v1/users/{regularUserId}/reviews");
        var adminUserResponse = await WebApplication.HttpClient.GetAsync($"api/v1/users/{adminUserId}/reviews");

        // Assert
        regularUserResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        adminUserResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        var regularUserResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(regularUserResponse);
        var adminUserResult = await DeserializeResponse<PagedResult<IEnumerable<BookReviewDto>>>(adminUserResponse);

        regularUserResult.ShouldNotBeNull();
        adminUserResult.ShouldNotBeNull();

        // Regular user should have 3 reviews, admin should have 2
        regularUserResult.Value.Count().ShouldBe(3);
        adminUserResult.Value.Count().ShouldBe(2);

        // Verify all reviews belong to the correct user
        regularUserResult.Value.All(r => r.UserId == regularUserId).ShouldBeTrue();
        adminUserResult.Value.All(r => r.UserId == adminUserId).ShouldBeTrue();

        // Verify no overlap between user reviews
        var regularReviewIds = regularUserResult.Value.Select(r => r.Id).ToList();
        var adminReviewIds = adminUserResult.Value.Select(r => r.Id).ToList();
        regularReviewIds.Intersect(adminReviewIds).ShouldBeEmpty();
    }
}