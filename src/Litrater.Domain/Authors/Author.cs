using Litrater.Domain.Books;
using Litrater.Domain.Common;

namespace Litrater.Domain.Authors;

public sealed class Author : AggregateRoot
{
    private readonly List<Book> _books = [];

#pragma warning disable CS8618 // Required by Entity Framework
    private Author() { }

    public Author(Guid id, PersonName name) : base(id)
    {
        Name = name;
    }

    public PersonName Name { get; private set; }
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    public void Update(PersonName name, List<Book> books)
    {
        Name = name;
        _books.Clear();
        _books.AddRange(books);
    }
}