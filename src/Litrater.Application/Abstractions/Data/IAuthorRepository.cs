using Litrater.Domain.Authors;

namespace Litrater.Application.Abstractions.Data;

public interface IAuthorRepository : IRepository
{
    Task<Author?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Author>> GetAuthorsByIdsAsync(IEnumerable<Guid> authorIds, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string firstName, string lastName, CancellationToken cancellationToken = default);
    Task AddAsync(Author author, CancellationToken cancellationToken = default);
    void Delete(Author author);
}