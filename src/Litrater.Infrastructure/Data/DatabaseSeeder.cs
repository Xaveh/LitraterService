using Litrater.Domain.Authors;
using Litrater.Domain.Books;
using Litrater.Domain.Users;
using Litrater.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedDataAsync(LitraterDbContext context)
    {
        await SeedUsersAsync(context);
        var authors = await SeedAuthorsAsync(context);
        await SeedBooksAsync(context, authors);
        await context.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(LitraterDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var passwordHasher = new PasswordHasher();

        var users = new[]
        {
            new User(
                new Guid("11111111-1111-1111-1111-111111111111"),
                "admin@litrater.com",
                passwordHasher.Hash("admin123"),
                "Admin",
                "User",
                UserRole.Admin
            ),
            new User(
                new Guid("22222222-2222-2222-2222-222222222222"),
                "user@litrater.com",
                passwordHasher.Hash("user123"),
                "Regular",
                "User",
                UserRole.User
            )
        };

        await context.Users.AddRangeAsync(users);
    }

    private static async Task<Dictionary<Guid, Author>> SeedAuthorsAsync(LitraterDbContext context)
    {
        if (await context.Authors.AnyAsync())
            return [];

        var authors = new[]
        {
            new Author(new Guid("31111111-1111-1111-1111-111111111111"), "J.K.", "Rowling"),
            new Author(new Guid("32222222-2222-2222-2222-222222222222"), "George", "Orwell"),
            new Author(new Guid("33333333-3333-3333-3333-333333333333"), "Harper", "Lee"),
            new Author(new Guid("34444444-4444-4444-4444-444444444444"), "F. Scott", "Fitzgerald"),
            new Author(new Guid("35555555-5555-5555-5555-555555555555"), "Jane", "Austen"),
            new Author(new Guid("36666666-6666-6666-6666-666666666666"), "Ernest", "Hemingway"),
            new Author(new Guid("37777777-7777-7777-7777-777777777777"), "J.R.R.", "Tolkien"),
            new Author(new Guid("38888888-8888-8888-8888-888888888888"), "Ray", "Bradbury"),
            new Author(new Guid("39999999-9999-9999-9999-999999999999"), "Aldous", "Huxley"),
            new Author(new Guid("3aaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Margaret", "Atwood")
        };

        await context.Authors.AddRangeAsync(authors);

        return authors.ToDictionary(author => author.Id);
    }
    private static async Task SeedBooksAsync(LitraterDbContext context, Dictionary<Guid, Author> authors)
    {
        if (await context.Books.AnyAsync())
            return;

        var books = new[]
        {
            new Book(new Guid("41111111-1111-1111-1111-111111111111"), "Harry Potter and the Philosopher's Stone", "9780747532699", [authors[new Guid("31111111-1111-1111-1111-111111111111")]]),
            new Book(new Guid("42222222-2222-2222-2222-222222222222"), "1984", "9780451524935", [authors[new Guid("32222222-2222-2222-2222-222222222222")]]),
            new Book(new Guid("43333333-3333-3333-3333-333333333333"), "To Kill a Mockingbird", "9780061120084", [authors[new Guid("33333333-3333-3333-3333-333333333333")]]),
            new Book(new Guid("44444444-4444-4444-4444-444444444444"), "The Great Gatsby", "9780743273565", [authors[new Guid("34444444-4444-4444-4444-444444444444")]]),
            new Book(new Guid("45555555-5555-5555-5555-555555555555"), "Pride and Prejudice", "9780141439518", [authors[new Guid("35555555-5555-5555-5555-555555555555")]]),
            new Book(new Guid("46666666-6666-6666-6666-666666666666"), "The Old Man and the Sea", "9780684801223", [authors[new Guid("36666666-6666-6666-6666-666666666666")]]),
            new Book(new Guid("47777777-7777-7777-7777-777777777777"), "The Hobbit", "9780547928227", [authors[new Guid("37777777-7777-7777-7777-777777777777")]]),
            new Book(new Guid("48888888-8888-8888-8888-888888888888"), "Fahrenheit 451", "9781451673319", [authors[new Guid("38888888-8888-8888-8888-888888888888")]]),
            new Book(new Guid("49999999-9999-9999-9999-999999999999"), "Brave New World", "9780060850524", [authors[new Guid("39999999-9999-9999-9999-999999999999")]]),
            new Book(new Guid("4aaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "The Handmaid's Tale", "9780385490818", [authors[new Guid("3aaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")]])
        };

        await context.Books.AddRangeAsync(books);
    }
}
