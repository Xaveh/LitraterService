using FluentValidation;

namespace Litrater.Application.Features.Authors.Queries.GetAuthorById;

internal sealed class GetAuthorByIdQueryValidator : AbstractValidator<GetAuthorByIdQuery>
{
    public GetAuthorByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}