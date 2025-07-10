using Litrater.Domain.Books;
using Litrater.Domain.Common;

namespace Litrater.Domain.Authors;

public sealed class Author : AggregateRoot
{
    private readonly List<Book> _books = [];

#pragma warning disable CS8618 // Required by Entity Framework
    private Author() { }

    public Author(Guid id, string firstName, string lastName) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    public void Update(string firstName, string lastName, List<Book> books)
    {
        FirstName = firstName;
        LastName = lastName;
        _books.Clear();
        _books.AddRange(books);
        SetModifiedDate();
    }
}