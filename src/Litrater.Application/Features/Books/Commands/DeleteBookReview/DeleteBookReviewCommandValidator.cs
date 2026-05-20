using FluentValidation;

namespace Litrater.Application.Features.Books.Commands.DeleteBookReview;

internal sealed class DeleteBookReviewCommandValidator : AbstractValidator<DeleteBookReviewCommand>
{
    public DeleteBookReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Book review ID must not be empty.");

        RuleFor(x => x.KeycloakUserId)
            .NotEmpty()
            .WithMessage("Keycloak user ID must not be empty.");
    }
}