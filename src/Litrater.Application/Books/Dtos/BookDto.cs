namespace Litrater.Application.Books.Dtos;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public IEnumerable<Guid> AuthorIds { get; set; } = [];
    public IEnumerable<Guid> ReviewIds { get; set; } = [];
}
