using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authors.Queries.GetAuthorById;
using Litrater.Domain.Authors;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Authors;

public sealed class GetAuthorByIdQueryHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly GetAuthorByIdQueryHandler _handler;

    public GetAuthorByIdQueryHandlerTests()
    {
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _handler = new GetAuthorByIdQueryHandler(_authorRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAuthorExists_ShouldReturnAuthorDto()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var query = new GetAuthorByIdQuery(authorId);
        var author = new Author(authorId, "John", "Doe");

        _authorRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(authorId);
        result.Value.FirstName.ShouldBe("John");
        result.Value.LastName.ShouldBe("Doe");
        result.Value.BookIds.ShouldNotBeNull();
        _authorRepositoryMock.Verify(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAuthorDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var query = new GetAuthorByIdQuery(authorId);

        _authorRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
    }
}