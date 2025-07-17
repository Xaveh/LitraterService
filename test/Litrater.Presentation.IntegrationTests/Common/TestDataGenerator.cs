using Litrater.Domain.Authors;
using Litrater.Domain.Books;
using Litrater.Domain.Users;

namespace Litrater.Presentation.IntegrationTests.Common;

public static class TestDataGenerator
{
    public static class Users
    {
        public static User Admin => new(
            new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
            new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

        public static User Regular => new(
            new Guid("b2c3d4e5-f617-4901-bcde-f23456789012"),
            new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
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

        public static Book Foundation =>
            new(new Guid("9b1c2d3e-4f5a-6b7c-8d9e-0f1a2b3c4d5e"),
                "Foundation",
                "9780553293357",
                [
                    Authors.Asimov
                ]);
    }

    public static class Authors
    {
        public static Author Tolkien => new(new Guid("16cf3c8d-fa5b-418a-ac3f-2ab55ebcb8b9"), "J.R.R.", "Tolkien");
        public static Author Rowling => new(new Guid("1d36391f-5d03-4a1d-8c07-0ff35f421bdb"), "J.K.", "Rowling");
        public static Author Herbert => new(new Guid("24610d22-961d-4a06-b2c3-40aaf247f3c8"), "Frank", "Herbert");
        public static Author Asimov => new(new Guid("41120f1c-c0e6-41ff-8bb5-9f5c5ec85a88"), "Isaac", "Asimov");
    }

    public static class BookReviews
    {
        public static BookReview HobbitReview1 => new(
            new Guid("c1d2e3f4-5678-9012-3456-789012345678"),
            "Amazing adventure story! Tolkien's world-building is incredible.",
            5,
            Books.TheHobbit.Id,
            Users.Regular.Id);

        public static BookReview HobbitReview2 => new(
            new Guid("d2e3f4a5-6789-0123-4567-890123456789"),
            "A classic fantasy tale that never gets old.",
            4,
            Books.TheHobbit.Id,
            Users.Admin.Id);

        public static BookReview HarryPotterReview1 => new(
            new Guid("e3f4a5b6-7890-1234-5678-901234567890"),
            "Magical story that captivated me from start to finish.",
            5,
            Books.HarryPotter.Id,
            Users.Regular.Id);

        public static BookReview DuneReview1 => new(
            new Guid("f4a5b6c7-8901-2345-6789-012345678901"),
            "Complex and thought-provoking science fiction masterpiece.",
            4,
            Books.Dune.Id,
            Users.Admin.Id);

        public static BookReview DuneReview2 => new(
            new Guid("a5b6c7d8-9012-3456-7890-123456789012"),
            "Dense but rewarding read. The world-building is phenomenal.",
            5,
            Books.Dune.Id,
            Users.Regular.Id);
    }
}