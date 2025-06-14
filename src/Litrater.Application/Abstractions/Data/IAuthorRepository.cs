using Litrater.Domain.Authors;

namespace Litrater.Application.Abstractions.Data;

public interface IAuthorRepository : IRepository
{
    Task<List<Author>> GetAuthorsByIdsAsync(IEnumerable<Guid> authorIds, CancellationToken cancellationToken = default);
}