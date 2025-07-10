using FluentValidation;

namespace Litrater.Application.Features.Books.Commands.CreateBook;

internal sealed class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Isbn)
            .NotEmpty()
            .WithMessage("ISBN is required")
            .Matches(@"^\d{13}$")
            .WithMessage("ISBN must be exactly 13 digits.");

        RuleFor(x => x.AuthorIds)
            .NotEmpty()
            .WithMessage("Author IDs must not be empty.");

        RuleForEach(x => x.AuthorIds)
            .NotEmpty()
            .WithMessage("Author IDs must not be empty.");
    }
}