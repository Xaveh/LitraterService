using FluentValidation;

namespace Litrater.Application.Features.Books.Commands.UpdateBook;

public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Book ID is required");

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