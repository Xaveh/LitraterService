using FluentValidation;

namespace Litrater.Application.Features.Books.Queries.GetBookById;

internal sealed class GetBookByIdQueryValidator : AbstractValidator<GetBookByIdQuery>
{
    public GetBookByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Book ID is required");
    }
}