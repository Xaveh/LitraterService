using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Books;

internal sealed class BookRepository(LitraterDbContext context) : Repository<Book>(context), IBookRepository
{
    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Reviews)
            .Include(b => b.Authors)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<(List<Book> Books, int TotalCount)> GetBooksAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(b => b.Reviews)
            .Include(b => b.Authors)
            .OrderBy(b => b.Title);

        var totalCount = await query.CountAsync(cancellationToken);

        var books = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (books, totalCount);
    }

    public async Task AddAsync(Book book, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(book, cancellationToken);
    }

    public Task<bool> ExistsByIsbnAsync(string isbn, CancellationToken cancellationToken = default)
    {
        return DbSet.AnyAsync(b => b.Isbn == isbn, cancellationToken);
    }

    public void Delete(Book book)
    {
        DbSet.Remove(book);
    }
}