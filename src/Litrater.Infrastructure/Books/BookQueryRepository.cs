using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Books;

internal sealed class BookQueryRepository(LitraterDbContext context) : QueryRepository<Book>(context), IBookQueryRepository
{
    public async Task<BookDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Reviews)
            .Include(b => b.Authors)
            .AsSplitQuery()
            .Where(b => b.Id == id)
            .Select(book => book.ToDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(IEnumerable<BookDto> Books, int TotalCount)> GetBooksAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalCount = await DbSet.CountAsync(cancellationToken);

        var books = await DbSet
            .Include(b => b.Reviews)
            .Include(b => b.Authors)
            .AsSplitQuery()
            .OrderBy(b => b.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(book => book.ToDto())
            .ToListAsync(cancellationToken);

        return (books, totalCount);
    }
}