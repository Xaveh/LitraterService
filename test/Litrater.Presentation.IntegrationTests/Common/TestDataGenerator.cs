using Litrater.Domain.Authors;
using Litrater.Domain.Books;
using Litrater.Domain.Users;

namespace Litrater.Presentation.IntegrationTests.Common;

public static class TestDataGenerator
{
    public static Author CreateAuthor(string? firstName = null, string? lastName = null)
    {
        return new Author(
            id: Guid.NewGuid(),
            firstName: firstName ?? "Test",
            lastName: lastName ?? "Author"
        );
    }

    public static User CreateUser(
        string? email = null,
        string? passwordHash = null,
        string? firstName = null,
        string? lastName = null,
        UserRole userRole = UserRole.User,
        bool isActive = true)
    {
        return new User(
            id: Guid.NewGuid(),
            email: email ?? "test@example.com",
            passwordHash: passwordHash ?? "hashedpassword",
            firstName: firstName ?? "Test",
            lastName: lastName ?? "User",
            isActive: isActive,
            userRole: userRole
        );
    }

    public static User CreateAdminUser(
        string? email = null,
        string? passwordHash = null,
        string? firstName = null,
        string? lastName = null)
    {
        return CreateUser(
            email: email ?? "admin@litrater.com",
            passwordHash: passwordHash,
            firstName: firstName ?? "Admin",
            lastName: lastName ?? "User",
            userRole: UserRole.Admin
        );
    }


    public static class Books
    {
        public static Book TheHobbit => CreateBook(
            title: "The Hobbit",
            isbn: "9780547928227",
            authors: [CreateAuthor("J.R.R.", "Tolkien")]
        );

        public static Book HarryPotter => CreateBook(
            title: "Harry Potter and the Philosopher's Stone",
            isbn: "9780747532699",
            authors: [CreateAuthor("J.K.", "Rowling")]
        );

        public static Book Dune => CreateBook(
            title: "Dune",
            isbn: "9780441172719",
            authors: [CreateAuthor("Frank", "Herbert")]
        );

        private static Book CreateBook(
            string? title = null,
            string? isbn = null,
            List<Author>? authors = null)
        {
            authors ??= [CreateAuthor()];

            return new Book(
                id: Guid.NewGuid(),
                title: title ?? "Test Book",
                isbn: isbn ?? GenerateIsbn(),
                authors: authors
            );
        }

        private static string GenerateIsbn()
        {
            var random = new Random();
            return $"978{random.Next(1000000000):D10}";
        }
    }

    public static class Authors
    {
        public static Author Tolkien => CreateAuthor("J.R.R.", "Tolkien");
        public static Author Rowling => CreateAuthor("J.K.", "Rowling");
        public static Author Herbert => CreateAuthor("Frank", "Herbert");
        public static Author Asimov => CreateAuthor("Isaac", "Asimov");
    }
}