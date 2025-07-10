using FluentValidation;

namespace Litrater.Application.Features.Books.Commands.CreateBookReview;

internal sealed class CreateBookReviewCommandValidator : AbstractValidator<CreateBookReviewCommand>
{
    public CreateBookReviewCommandValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage("Content is required")
            .MaximumLength(1000)
            .WithMessage("Content cannot exceed 1000 characters");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.BookId)
            .NotEmpty()
            .WithMessage("Book ID must not be empty.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID must not be empty.");
    }
}