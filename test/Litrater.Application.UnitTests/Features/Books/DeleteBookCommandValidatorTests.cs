using FluentValidation.TestHelper;
using Litrater.Application.Features.Books.Commands.DeleteBook;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class DeleteBookCommandValidatorTests
{
    private readonly DeleteBookCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsValid_ShouldBeValid()
    {
        // Arrange
        var command = new DeleteBookCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldBeInvalid()
    {
        // Arrange
        var command = new DeleteBookCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Book ID is required");
    }
}