using FluentValidation;

namespace Litrater.Application.Features.Books.Commands.DeleteBookReview;

public sealed class DeleteBookReviewCommandValidator : AbstractValidator<DeleteBookReviewCommand>
{
    public DeleteBookReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Book review ID must not be empty.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID must not be empty.");
    }
} 