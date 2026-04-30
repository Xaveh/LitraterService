using Litrater.Domain.Authors;

namespace Litrater.Application.Abstractions.Data;

public interface IAuthorCommandRepository : ICommandRepository<Author>
{
    Task<List<Author>> GetAuthorsByIdsAsync(IEnumerable<Guid> authorIds, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default);
}