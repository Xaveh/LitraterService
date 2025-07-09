using Litrater.Domain.Authors;
using Litrater.Domain.Books;
using Litrater.Domain.Users;
using Litrater.Infrastructure.Authentication;

namespace Litrater.Presentation.IntegrationTests.Common;

public static class TestDataGenerator
{
    private static readonly PasswordHasher PasswordHasher = new();

    public static class Users
    {
        public static User Admin => new(
            new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            "admin@litrater.com",
            PasswordHasher.Hash("admin123"),
            "Admin",
            "User",
            true,
            UserRole.Admin);

        public static User Regular => new(
            new Guid("b2c3d4e5-f617-4901-bcde-f23456789012"),
            "user@litrater.com",
            PasswordHasher.Hash("user123"),
            "Regular",
            "User");
    }

    public static class Books
    {
        public static Book TheHobbit =>
            new(new Guid("b4027480-3875-4cfe-8f72-298e31228aed"),
                "The Hobbit",
                "9780547928227",
                [
                    Authors.Tolkien
                ]);

        public static Book HarryPotter =>
            new(new Guid("17536d6c-e82c-4514-b99e-3a825a58b1b1"),
                "Harry Potter and the Philosopher's Stone",
                "9780747532699",
                [
                    Authors.Rowling
                ]);

        public static Book Dune =>
            new(new Guid("8a043fa2-7573-4d4d-bc7a-7fd85a16cbd5"),
                "Dune",
                "9780441172719",
                [
                    Authors.Herbert
                ]);
    }

    public static class Authors
    {
        public static Author Tolkien => new(new Guid("16cf3c8d-fa5b-418a-ac3f-2ab55ebcb8b9"), "J.R.R.", "Tolkien");
        public static Author Rowling => new(new Guid("1d36391f-5d03-4a1d-8c07-0ff35f421bdb"), "J.K.", "Rowling");
        public static Author Herbert => new(new Guid("24610d22-961d-4a06-b2c3-40aaf247f3c8"), "Frank", "Herbert");
        public static Author Asimov => new(new Guid("41120f1c-c0e6-41ff-8bb5-9f5c5ec85a88"), "Isaac", "Asimov");
    }
}