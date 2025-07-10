using System.Net;
using System.Net.Http.Json;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.BookReviews;

public class UpdateBookReviewEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task UpdateBookReview_WithValidDataAndOwnerAuthentication_ShouldUpdateBookReview()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id; // Regular user owns this review
        var updateBookReviewRequest = new
        {
            Content = "Updated review content - even more amazing!",
            Rating = 4
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/book-reviews/{bookReviewId}", updateBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var bookReviewDto = await DeserializeResponse<BookReviewDto>(response);

        bookReviewDto.ShouldNotBeNull();
        bookReviewDto.Id.ShouldBe(bookReviewId);
        bookReviewDto.Content.ShouldBe(updateBookReviewRequest.Content);
        bookReviewDto.Rating.ShouldBe(updateBookReviewRequest.Rating);
        bookReviewDto.BookId.ShouldBe(TestDataGenerator.Books.TheHobbit.Id);
        bookReviewDto.UserId.ShouldBe(TestDataGenerator.Users.Regular.Id);

        // Clear the change tracker to ensure we get fresh data from the database
        WebApplication.DbContext.ChangeTracker.Clear();

        var persistedBookReview = await WebApplication.DbContext.BookReviews
            .FirstOrDefaultAsync(br => br.Id == bookReviewId);

        persistedBookReview.ShouldNotBeNull();
        persistedBookReview.Content.ShouldBe(updateBookReviewRequest.Content);
        persistedBookReview.Rating.ShouldBe(updateBookReviewRequest.Rating);
        persistedBookReview.UserId.ShouldBe(TestDataGenerator.Users.Regular.Id); // Should remain unchanged
    }

    [Fact]
    public async Task UpdateBookReview_WithAdminAuthentication_ShouldUpdateAnyBookReview()
    {
        // Arrange
        await LoginAsAdminAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id; // Regular user owns this review
        var updateBookReviewRequest = new
        {
            Content = "Admin updated this review content",
            Rating = 3
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/book-reviews/{bookReviewId}", updateBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var bookReviewDto = await DeserializeResponse<BookReviewDto>(response);

        bookReviewDto.ShouldNotBeNull();
        bookReviewDto.Id.ShouldBe(bookReviewId);
        bookReviewDto.Content.ShouldBe(updateBookReviewRequest.Content);
        bookReviewDto.Rating.ShouldBe(updateBookReviewRequest.Rating);
        bookReviewDto.BookId.ShouldBe(TestDataGenerator.Books.TheHobbit.Id);
        bookReviewDto.UserId.ShouldBe(TestDataGenerator.Users.Regular.Id); // Original owner should remain

        // Clear the change tracker to ensure we get fresh data from the database
        WebApplication.DbContext.ChangeTracker.Clear();

        var persistedBookReview = await WebApplication.DbContext.BookReviews
            .FirstOrDefaultAsync(br => br.Id == bookReviewId);

        persistedBookReview.ShouldNotBeNull();
        persistedBookReview.Content.ShouldBe(updateBookReviewRequest.Content);
        persistedBookReview.Rating.ShouldBe(updateBookReviewRequest.Rating);
        persistedBookReview.UserId.ShouldBe(TestDataGenerator.Users.Regular.Id); // Should remain unchanged
    }

    [Fact]
    public async Task UpdateBookReview_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id;
        var updateBookReviewRequest = new
        {
            Content = "Unauthorized update attempt",
            Rating = 2
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/book-reviews/{bookReviewId}", updateBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UpdateBookReview_WhenUserIsNotOwner_ShouldReturnForbidden()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview2.Id; // Admin owns this review
        var updateBookReviewRequest = new
        {
            Content = "Trying to update someone else's review",
            Rating = 1
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/book-reviews/{bookReviewId}", updateBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task UpdateBookReview_WithNonExistentBookReview_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var nonExistentBookReviewId = Guid.NewGuid();
        var updateBookReviewRequest = new
        {
            Content = "Trying to update non-existent review",
            Rating = 3
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/book-reviews/{nonExistentBookReviewId}", updateBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateBookReview_WithEmptyContent_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id;
        var updateBookReviewRequest = new
        {
            Content = "", // Empty content
            Rating = 4
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/book-reviews/{bookReviewId}", updateBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateBookReview_WithInvalidRating_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id;
        var updateBookReviewRequest = new
        {
            Content = "Valid content",
            Rating = 6 // Invalid rating (should be 1-5)
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/book-reviews/{bookReviewId}", updateBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateBookReview_WithTooLongContent_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview1.Id;
        var longContent = new string('a', 1001); // Exceeds 1000 character limit
        var updateBookReviewRequest = new
        {
            Content = longContent,
            Rating = 4
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/book-reviews/{bookReviewId}", updateBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateBookReview_AdminUpdatingOwnReview_ShouldUpdateSuccessfully()
    {
        // Arrange
        await LoginAsAdminAsync();

        var bookReviewId = TestDataGenerator.BookReviews.HobbitReview2.Id; // Admin owns this review
        var updateBookReviewRequest = new
        {
            Content = "Admin updating own review",
            Rating = 5
        };

        // Act
        var response = await WebApplication.HttpClient.PutAsJsonAsync($"api/v1/book-reviews/{bookReviewId}", updateBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var bookReviewDto = await DeserializeResponse<BookReviewDto>(response);

        bookReviewDto.ShouldNotBeNull();
        bookReviewDto.Id.ShouldBe(bookReviewId);
        bookReviewDto.Content.ShouldBe(updateBookReviewRequest.Content);
        bookReviewDto.Rating.ShouldBe(updateBookReviewRequest.Rating);
        bookReviewDto.UserId.ShouldBe(TestDataGenerator.Users.Admin.Id);
    }
}