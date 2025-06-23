using Litrater.Application.Abstractions.Authentication;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Litrater.Presentation.IntegrationTests.Common;

internal static class DatabaseSeeder
{
    public static async Task SeedTestDataAsync(IServiceScope serviceScope)
    {
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<LitraterDbContext>();
        var passwordHasher = serviceScope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await SeedUsersAsync(dbContext, passwordHasher);
        await SeedAuthorsAsync(dbContext);
        await SeedBooksAsync(dbContext);
    }

    private static async Task SeedUsersAsync(LitraterDbContext dbContext, IPasswordHasher passwordHasher)
    {
        if (await dbContext.Users.AnyAsync())
            return;

        var adminUser = TestDataGenerator.CreateAdminUser(
            email: "admin@litrater.com",
            passwordHash: passwordHasher.Hash("admin123"),
            firstName: "Admin",
            lastName: "User"
        );

        var regularUser = TestDataGenerator.CreateUser(
            email: "user@litrater.com",
            passwordHash: passwordHasher.Hash("user123"),
            firstName: "Regular",
            lastName: "User"
        );

        dbContext.Users.AddRange(adminUser, regularUser);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedAuthorsAsync(LitraterDbContext dbContext)
    {
        if (await dbContext.Authors.AnyAsync())
            return;

        var authors = new[]
        {
            TestDataGenerator.Authors.Tolkien,
            TestDataGenerator.Authors.Rowling,
            TestDataGenerator.Authors.Herbert,
            TestDataGenerator.Authors.Asimov
        };

        dbContext.Authors.AddRange(authors);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedBooksAsync(LitraterDbContext dbContext)
    {
        if (await dbContext.Books.AnyAsync())
            return;

        var tolkien = await dbContext.Authors.FirstAsync(a => a.LastName == "Tolkien");
        var rowling = await dbContext.Authors.FirstAsync(a => a.LastName == "Rowling");
        var herbert = await dbContext.Authors.FirstAsync(a => a.LastName == "Herbert");

        var books = new[]
        {
            new Book(Guid.NewGuid(), "The Hobbit", "9780547928227", [tolkien]),
            new Book(Guid.NewGuid(), "Harry Potter and the Philosopher's Stone", "9780747532699", [rowling]),
            new Book(Guid.NewGuid(), "Dune", "9780441172719", [herbert])
        };

        dbContext.Books.AddRange(books);
        await dbContext.SaveChangesAsync();
    }
}