using FluentValidation.TestHelper;
using Litrater.Application.Features.Authors.Commands.DeleteAuthor;

namespace Litrater.Application.UnitTests.Features.Authors;

public sealed class DeleteAuthorCommandValidatorTests
{
    private readonly DeleteAuthorCommandValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsValid_ShouldBeValid()
    {
        // Arrange
        var command = new DeleteAuthorCommand(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldBeInvalid()
    {
        // Arrange
        var command = new DeleteAuthorCommand(Guid.Empty);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}