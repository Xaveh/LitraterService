using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Domain.Authors;
using Litrater.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Litrater.Infrastructure.Authors;

internal sealed class AuthorQueryRepository(LitraterDbContext context) : QueryRepository<Author>(context), IAuthorQueryRepository
{
    public async Task<AuthorDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(a => a.Books)
            .Where(a => a.Id == id)
            .Select(AuthorDtoExtensions.Projection)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
