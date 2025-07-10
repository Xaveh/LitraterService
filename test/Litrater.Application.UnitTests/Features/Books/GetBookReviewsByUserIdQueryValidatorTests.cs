using FluentValidation.TestHelper;
using Litrater.Application.Features.Books.Queries.GetBookReviewsByUserId;

namespace Litrater.Application.UnitTests.Features.Books;

public class GetBookReviewsByUserIdQueryValidatorTests
{
    private readonly GetBookReviewsByUserIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WithValidQuery_ShouldNotHaveValidationErrors()
    {
        // Arrange
        var query = new GetBookReviewsByUserIdQuery(Guid.NewGuid(), 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_WithEmptyUserId_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetBookReviewsByUserIdQuery(Guid.Empty, 1, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId)
            .WithErrorMessage("UserId is required");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_WithInvalidPage_ShouldHaveValidationError(int page)
    {
        // Arrange
        var query = new GetBookReviewsByUserIdQuery(Guid.NewGuid(), page, 10);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Page)
            .WithErrorMessage("Page must be greater than 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_WithInvalidPageSize_ShouldHaveValidationError(int pageSize)
    {
        // Arrange
        var query = new GetBookReviewsByUserIdQuery(Guid.NewGuid(), 1, pageSize);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must be greater than 0");
    }

    [Fact]
    public void Validate_WithPageSizeExceedingLimit_ShouldHaveValidationError()
    {
        // Arrange
        var query = new GetBookReviewsByUserIdQuery(Guid.NewGuid(), 1, 101);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize)
            .WithErrorMessage("PageSize must not exceed 100");
    }

    [Fact]
    public void Validate_WithPageSizeAtLimit_ShouldNotHaveValidationError()
    {
        // Arrange
        var query = new GetBookReviewsByUserIdQuery(Guid.NewGuid(), 1, 100);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(5, 25)]
    [InlineData(10, 50)]
    [InlineData(100, 100)]
    public void Validate_WithValidPageAndPageSize_ShouldNotHaveValidationErrors(int page, int pageSize)
    {
        // Arrange
        var query = new GetBookReviewsByUserIdQuery(Guid.NewGuid(), page, pageSize);

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
} 