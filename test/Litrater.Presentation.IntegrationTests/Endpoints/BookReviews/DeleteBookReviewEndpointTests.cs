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
        await LoginAsRegularUserAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id; // Regular user owns this review

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{bookReviewId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Clear the change tracker to ensure we get fresh data from the database
        WebApplication.DbContext.ChangeTracker.Clear();

        var deletedBookReview = await WebApplication.DbContext.BookReviews
            .FirstOrDefaultAsync(br => br.Id == bookReviewId);

        deletedBookReview.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteBookReview_WithAdminAuthentication_ShouldDeleteAnyBookReview()
    {
        // Arrange
        await LoginAsAdminAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id; // Regular user owns this review

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{bookReviewId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Clear the change tracker to ensure we get fresh data from the database
        WebApplication.DbContext.ChangeTracker.Clear();

        var deletedBookReview = await WebApplication.DbContext.BookReviews
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
        await LoginAsRegularUserAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview2.Id; // Admin owns this review

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{bookReviewId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteBookReview_WithNonExistentBookReview_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var nonExistentBookReviewId = Guid.NewGuid();

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{nonExistentBookReviewId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBookReview_AdminDeletingOwnReview_ShouldDeleteSuccessfully()
    {
        // Arrange
        await LoginAsAdminAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview2.Id; // Admin owns this review

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{bookReviewId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Clear the change tracker to ensure we get fresh data from the database
        WebApplication.DbContext.ChangeTracker.Clear();

        var deletedBookReview = await WebApplication.DbContext.BookReviews
            .FirstOrDefaultAsync(br => br.Id == bookReviewId);

        deletedBookReview.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteBookReview_WithInvalidGuid_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var invalidGuid = "invalid-guid";

        // Act
        var response = await WebApplication.HttpClient.DeleteAsync($"api/v1/book-reviews/{invalidGuid}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}