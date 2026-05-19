using FluentValidation;

namespace Litrater.Application.Features.Books.Commands.DeleteBook;

internal sealed class DeleteBookCommandValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Book ID is required");
    }
}