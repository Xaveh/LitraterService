using System.Net;
using System.Net.Http.Json;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Presentation.IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Litrater.Presentation.IntegrationTests.Endpoints.BookReviews;

public class CreateBookReviewEndpointTests(DatabaseFixture fixture) : BaseIntegrationTest(fixture)
{
    [Fact]
    public async Task CreateBookReview_WithValidDataAndAuthentication_ShouldCreateBookReview()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var bookId = TestDataGenerator.Books.Foundation.Id; // Regular user hasn't reviewed Foundation

        var createBookReviewRequest = new
        {
            Content = "An amazing adventure story!",
            Rating = 5
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync($"api/v1/books/{bookId}/reviews", createBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var bookReviewDto = await DeserializeResponse<BookReviewDto>(response);

        bookReviewDto.ShouldNotBeNull();
        bookReviewDto.Content.ShouldBe(createBookReviewRequest.Content);
        bookReviewDto.Rating.ShouldBe(createBookReviewRequest.Rating);
        bookReviewDto.BookId.ShouldBe(bookId);
        bookReviewDto.UserId.ShouldBe(TestDataGenerator.Users.Regular.Id);

        var persistedBookReview = await WebApplication.DbContext.BookReviews
            .FirstOrDefaultAsync(br => br.Id == bookReviewDto.Id);

        persistedBookReview.ShouldNotBeNull();
        persistedBookReview.Content.ShouldBe(createBookReviewRequest.Content);
        persistedBookReview.Rating.ShouldBe(createBookReviewRequest.Rating);
        persistedBookReview.BookId.ShouldBe(bookId);
        persistedBookReview.UserId.ShouldBe(TestDataGenerator.Users.Regular.Id);
    }

    [Fact]
    public async Task CreateBookReview_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var bookId = TestDataGenerator.Books.TheHobbit.Id;

        var createBookReviewRequest = new
        {
            Content = "An amazing adventure story!",
            Rating = 5
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync($"api/v1/books/{bookId}/reviews", createBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBookReview_WithNonExistentBook_ShouldReturnNotFound()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var nonExistentBookId = Guid.NewGuid();

        var createBookReviewRequest = new
        {
            Content = "An amazing adventure story!",
            Rating = 5
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync($"api/v1/books/{nonExistentBookId}/reviews", createBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateBookReview_WhenUserAlreadyReviewedBook_ShouldReturnConflict()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var bookId = TestDataGenerator.Books.TheHobbit.Id; // Regular user already has a review for TheHobbit

        var secondReviewRequest = new
        {
            Content = "Second review",
            Rating = 5
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync($"api/v1/books/{bookId}/reviews", secondReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task CreateBookReview_WithEmptyContent_ShouldReturnBadRequest()
    {
        // Arrange
        await LoginAsRegularUserAsync();

        var bookId = TestDataGenerator.Books.Foundation.Id; // Regular user hasn't reviewed Foundation

        var createBookReviewRequest = new
        {
            Content = "", // Empty content
            Rating = 5
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync($"api/v1/books/{bookId}/reviews", createBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}