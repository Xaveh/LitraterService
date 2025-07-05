using FluentValidation.TestHelper;
using Litrater.Application.Features.Books.Queries.GetBookById;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class GetBookByIdQueryValidatorTests
{
    private readonly GetBookByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsValid_ShouldBeValid()
    {
        // Arrange
        var query = new GetBookByIdQuery(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldBeInvalid()
    {
        // Arrange
        var query = new GetBookByIdQuery(Guid.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}