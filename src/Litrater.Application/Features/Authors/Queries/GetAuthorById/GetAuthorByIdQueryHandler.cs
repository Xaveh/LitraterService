using Ardalis.Result;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authors.Dtos;

namespace Litrater.Application.Features.Authors.Queries.GetAuthorById;

internal sealed class GetAuthorByIdQueryHandler(IAuthorQueryRepository authorQueryRepository) : IQueryHandler<GetAuthorByIdQuery, AuthorDto>
{
    public async Task<Result<AuthorDto>> Handle(GetAuthorByIdQuery query, CancellationToken cancellationToken)
    {
        var author = await authorQueryRepository.GetByIdAsync(query.Id, cancellationToken);

        return author is null ? Result<AuthorDto>.NotFound() : Result<AuthorDto>.Success(author);
    }
}