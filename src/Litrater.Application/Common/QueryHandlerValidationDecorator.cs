using Ardalis.Result;
using Ardalis.Result.FluentValidation;
using FluentValidation;
using Litrater.Application.Common.Interfaces;

namespace Litrater.Application.Common;

internal sealed class QueryHandlerValidationDecorator<TQuery, TResponse>(IQueryHandler<TQuery, TResponse> inner, IValidator<TQuery>? validator)
    : IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery
{
    public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        if (validator is not null)
        {
            var result = await validator.ValidateAsync(query, cancellationToken);
            if (!result.IsValid)
            {
                return Result<TResponse>.Invalid(result.AsErrors());
            }
        }

        return await inner.Handle(query, cancellationToken);
    }
}