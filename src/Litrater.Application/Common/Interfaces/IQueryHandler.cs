using Ardalis.Result;

namespace Litrater.Application.Common.Interfaces;

public interface IQueryHandler<in TQuery, TResponse>
    where TQuery : IQuery
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken);
}
