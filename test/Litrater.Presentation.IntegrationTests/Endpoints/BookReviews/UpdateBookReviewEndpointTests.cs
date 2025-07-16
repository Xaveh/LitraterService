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
        LoginAsRegularUserAsync();

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

        var persistedBookReview = await WebApplication.DbContext.BookReviews.AsNoTracking()
            .FirstOrDefaultAsync(br => br.Id == bookReviewId);

        persistedBookReview.ShouldNotBeNull();
        persistedBookReview.Content.ShouldBe(updateBookReviewRequest.Content);
        persistedBookReview.Rating.ShouldBe(updateBookReviewRequest.Rating);
        persistedBookReview.UserId.ShouldBe(TestDataGenerator.Users.Regular.Id); // Should remain unchanged
        persistedBookReview.ModifiedDate.ShouldNotBeNull();
        persistedBookReview.ModifiedDate.Value.ShouldBeGreaterThan(persistedBookReview.CreatedDate);
    }

    [Fact]
    public async Task UpdateBookReview_WithAdminAuthentication_ShouldUpdateAnyBookReview()
    {
        // Arrange
        LoginAsAdminAsync();

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

        var persistedBookReview = await WebApplication.DbContext.BookReviews.AsNoTracking()
            .FirstOrDefaultAsync(br => br.Id == bookReviewId);

        persistedBookReview.ShouldNotBeNull();
        persistedBookReview.Content.ShouldBe(updateBookReviewRequest.Content);
        persistedBookReview.Rating.ShouldBe(updateBookReviewRequest.Rating);
        persistedBookReview.UserId.ShouldBe(TestDataGenerator.Users.Regular.Id); // Should remain unchanged
        persistedBookReview.ModifiedDate.ShouldNotBeNull();
        persistedBookReview.ModifiedDate.Value.ShouldBeGreaterThan(persistedBookReview.CreatedDate);
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
        LoginAsRegularUserAsync();

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
    public async Task UpdateBookReview_AdminUpdatingOwnReview_ShouldUpdateSuccessfully()
    {
        // Arrange
        LoginAsAdminAsync();

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