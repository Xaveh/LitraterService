using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Presentation.IntegrationTests.Common;

internal static class DatabaseSeeder
{
    public static async Task SeedTestDataAsync(LitraterDbContext dbContext)
    {
        await SeedUsersAsync(dbContext);
        await SeedBooksAndAuthorsAsync(dbContext);
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
}