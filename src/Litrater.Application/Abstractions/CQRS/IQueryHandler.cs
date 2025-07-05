using Ardalis.Result;

namespace Litrater.Application.Abstractions.CQRS;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}