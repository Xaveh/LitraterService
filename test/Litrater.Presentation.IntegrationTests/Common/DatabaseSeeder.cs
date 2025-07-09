using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Presentation.IntegrationTests.Common;

internal static class DatabaseSeeder
{
    public static async Task SeedTestDataAsync(LitraterDbContext dbContext)
    {
        await SeedUsersAsync(dbContext);
        await SeedBooksAndAuthorsAsync(dbContext);
        await SeedBookReviewsAsync(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(LitraterDbContext dbContext)
    {
        if (await dbContext.Users.AnyAsync())
            return;

        dbContext.Users.AddRange(TestDataGenerator.Users.Admin, TestDataGenerator.Users.Regular);
    }

    private static async Task SeedBooksAndAuthorsAsync(LitraterDbContext dbContext)
    {
        if (await dbContext.Books.AnyAsync())
            return;

        var books = new[]
        {
            TestDataGenerator.Books.TheHobbit,
            TestDataGenerator.Books.HarryPotter,
            TestDataGenerator.Books.Dune
        };

        var authors = books.SelectMany(x => x.Authors).Distinct();

        dbContext.Authors.AddRange(authors);
        dbContext.Books.AddRange(books);
    }

    private static async Task SeedBookReviewsAsync(LitraterDbContext dbContext)
    {
        if (await dbContext.BookReviews.AnyAsync())
            return;

        var bookReviews = new[]
        {
            TestDataGenerator.BookReviews.HobbitReview1,
            TestDataGenerator.BookReviews.HobbitReview2,
            TestDataGenerator.BookReviews.HarryPotterReview1,
            TestDataGenerator.BookReviews.DuneReview1,
            TestDataGenerator.BookReviews.DuneReview2
        };

        dbContext.BookReviews.AddRange(bookReviews);
    }
}