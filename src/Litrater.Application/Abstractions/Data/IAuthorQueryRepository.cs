using Litrater.Application.Features.Authors.Dtos;

namespace Litrater.Application.Abstractions.Data;

public interface IAuthorQueryRepository : IQueryRepository
{
    Task<AuthorDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}