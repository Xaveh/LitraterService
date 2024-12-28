namespace Litrater.Domain.Models;

public sealed class Author : Entity
{
    private readonly List<Book> _books = [];

    public Author(Guid id, string name) : base(id)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public string Name { get; private set; }
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    public void AddBook(Book book)
    {
        if (!_books.Contains(book))
            _books.Add(book);
    }
}