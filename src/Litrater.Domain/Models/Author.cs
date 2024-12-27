namespace Litrater.Domain.Models;

public sealed class Author
{
    private readonly List<Book> _books = [];

    public Author(Guid id, string name)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    public void AddBook(Book book)
    {
        if (!_books.Contains(book))
            _books.Add(book);
    }
}