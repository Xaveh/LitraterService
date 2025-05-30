using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Books;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Books;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(LitraterDbContext context) : base(context)
    {
    }

    public async Task<Book?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(b => b.Reviews)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }
}