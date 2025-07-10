using FluentValidation;

namespace Litrater.Application.Features.Authors.Commands.UpdateAuthor;

public sealed class UpdateAuthorCommandValidator : AbstractValidator<UpdateAuthorCommand>
{
    public UpdateAuthorCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.BookIds)
            .NotEmpty()
            .WithMessage("Book IDs must not be empty.");

        RuleForEach(x => x.BookIds)
            .NotEmpty()
            .WithMessage("Book IDs must not be empty.");
    }
}