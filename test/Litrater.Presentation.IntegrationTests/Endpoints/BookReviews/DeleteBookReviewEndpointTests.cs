using System.Net;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.BookReviews;

public class DeleteBookReviewEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task DeleteBookReview_WithValidIdAndOwnerAuthentication_ShouldDeleteBookReview()
    {
        // Arrange
        LoginAsRegularUserAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id; // Regular user owns this review

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{bookReviewId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var deletedBookReview = await WebApplication.DbContext.BookReviews.AsNoTracking()
            .FirstOrDefaultAsync(br => br.Id == bookReviewId);

        deletedBookReview.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteBookReview_WithAdminAuthentication_ShouldDeleteAnyBookReview()
    {
        // Arrange
        LoginAsAdminAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id; // Regular user owns this review

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{bookReviewId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var deletedBookReview = await WebApplication.DbContext.BookReviews.AsNoTracking()
            .FirstOrDefaultAsync(br => br.Id == bookReviewId);

        deletedBookReview.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteBookReview_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id;

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{bookReviewId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteBookReview_WhenUserIsNotOwner_ShouldReturnForbidden()
    {
        // Arrange
        LoginAsRegularUserAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview2.Id; // Admin owns this review

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{bookReviewId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteBookReview_AdminDeletingOwnReview_ShouldDeleteSuccessfully()
    {
        // Arrange
        LoginAsAdminAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview2.Id; // Admin owns this review

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{bookReviewId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var deletedBookReview = await WebApplication.DbContext.BookReviews.AsNoTracking()
            .FirstOrDefaultAsync(br => br.Id == bookReviewId);

        deletedBookReview.ShouldBeNull();
    }
}