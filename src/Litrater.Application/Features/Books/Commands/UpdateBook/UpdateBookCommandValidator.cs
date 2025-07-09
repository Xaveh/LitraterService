using FluentValidation;

namespace Litrater.Application.Features.Books.Commands.UpdateBook;

public sealed class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Isbn)
            .NotEmpty()
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