using Litrater.Domain.Shared;

namespace Litrater.Domain.Models;

public sealed class Author : AggregateRoot
{
    private readonly List<Book> _books = [];

    #pragma warning disable CS8618 // Required by Entity Framework
    private Author() {}

    public Author(Guid id, string firstName, string lastName) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    public void AddBook(Book book)
    {
        if (!_books.Contains(book))
        {
            _books.Add(book);
        }
    }
}