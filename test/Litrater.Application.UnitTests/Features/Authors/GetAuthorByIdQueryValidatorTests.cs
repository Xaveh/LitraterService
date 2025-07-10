using FluentValidation.TestHelper;
using Litrater.Application.Features.Authors.Queries.GetAuthorById;

namespace Litrater.Application.UnitTests.Features.Authors;

public sealed class GetAuthorByIdQueryValidatorTests
{
    private readonly GetAuthorByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsValid_ShouldBeValid()
    {
        // Arrange
        var query = new GetAuthorByIdQuery(Guid.NewGuid());

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WhenIdIsEmpty_ShouldBeInvalid()
    {
        // Arrange
        var query = new GetAuthorByIdQuery(Guid.Empty);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}