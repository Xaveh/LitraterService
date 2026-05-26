using Litrater.Domain.Common;

namespace Litrater.Application.Abstractions.Data;

public interface ICommandRepository<T> where T : Entity
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}