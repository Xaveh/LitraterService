using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Microsoft.Extensions.Logging;

namespace Litrater.Application.Behaviors;

internal sealed class QueryHandlerLoggingDecorator<TQuery, TResponse>(
    IQueryHandler<TQuery, TResponse> innerHandler,
    ILogger<QueryHandlerLoggingDecorator<TQuery, TResponse>> logger)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery
{
    public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var queryName = typeof(TQuery).Name;

        logger.LogInformation("Handling query: {QueryType}", queryName);

        var result = await innerHandler.Handle(query, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogInformation("Query {QueryType} processed successfully.", queryName);
        }
        else
        {
            var allErrors = result.Errors.Concat(result.ValidationErrors.Select(x => x.ErrorMessage));
            logger.LogWarning("Query {QueryType} failed. Status: {Status}, Errors: {Errors}", queryName, result.Status,
                string.Join(", ", allErrors));
        }


        return result;
    }
}