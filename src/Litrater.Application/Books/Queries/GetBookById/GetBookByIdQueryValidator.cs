using FluentValidation;

namespace Litrater.Application.Books.Queries.GetBookById;

internal sealed class GetBookByIdQueryValidator : AbstractValidator<GetBookByIdQuery>
{
    public GetBookByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}