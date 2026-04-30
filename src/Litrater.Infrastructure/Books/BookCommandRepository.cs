using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Books;

internal sealed class BookCommandRepository(LitraterDbContext context) : CommandRepository<Book>(context), IBookCommandRepository
{
    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Reviews)
            .Include(b => b.Authors)
            .AsSplitQuery()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public Task<List<Book>> GetBooksByIdsAsync(IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(book => bookIds.Contains(book.Id))
            .Include(b => b.Reviews)
            .Include(b => b.Authors)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(book, cancellationToken);
    }

    public Task<bool> ExistsByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        var isbnValue = new Isbn(isbn);
        return DbSet.AnyAsync(b => b.Isbn == isbnValue, cancellationToken);
    }

    public void Delete(Book book)
    {
        DbSet.Remove(book);
    }
}