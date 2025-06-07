using FluentValidation;

namespace Litrater.Application.Features.Books.Commands.CreateBook;

internal sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Isbn)
            .NotEmpty()
            .Matches(@"^\d{13}$")
            .WithMessage("ISBN must be exactly 13 digits.");

        RuleFor(x => x.AuthorIds)
            .NotEmpty()
            .Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Author IDs must not be empty.");
    }
}
