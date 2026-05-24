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
        LoginAsRegularUserAsync();

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
        persistedBookReview.Rating.Value.ShouldBe(createBookReviewRequest.Rating);
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
    public async Task CreateBookReview_ShouldUpdateAverageRating()
    {
        // Arrange
        LoginAsRegularUserAsync();

        var bookId = TestDataGenerator.Books.Foundation.Id;

        var createBookReviewRequest = new
        {
            Content = "A masterpiece of science fiction.",
            Rating = 4
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync($"api/v1/books/{bookId}/reviews", createBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var book = await WebApplication.DbContext.Books.AsNoTracking()
            .FirstAsync(b => b.Id == bookId);

        book.AverageRating.ShouldBe(4.0);

        var getResponse = await WebApplication.HttpClient.GetAsync($"api/v1/books/{bookId}");
        var bookDto = await DeserializeResponse<BookDto>(getResponse);
        bookDto.AverageRating.ShouldBe(4.0);
    }

    [Fact]
    public async Task CreateBookReview_MultipleReviews_ShouldCalculateCorrectAverage()
    {
        // Arrange
        LoginAsAdminAsync();

        var bookId = TestDataGenerator.Books.HarryPotter.Id;

        var createBookReviewRequest = new
        {
            Content = "A magical masterpiece that set the standard for fantasy.",
            Rating = 4
        };

        // Act
        var response = await WebApplication.HttpClient.PostAsJsonAsync($"api/v1/books/{bookId}/reviews", createBookReviewRequest);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var book = await WebApplication.DbContext.Books.AsNoTracking()
            .FirstAsync(b => b.Id == bookId);

        book.AverageRating.ShouldNotBeNull();
        book.AverageRating.Value.ShouldBe(4.5);
    }
}