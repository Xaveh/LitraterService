using FluentValidation;

namespace Litrater.Application.Features.Authors.Commands.UpdateAuthor;

public sealed class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Author ID is required");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(100)
            .WithMessage("First name cannot exceed 100 characters");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(100)
            .WithMessage("Last name cannot exceed 100 characters");

        RuleFor(x => x.BookIds)
            .NotEmpty()
            .WithMessage("Book IDs must not be empty.");

        RuleForEach(x => x.BookIds)
            .NotEmpty()
            .WithMessage("Book IDs must not be empty.");
    }
}