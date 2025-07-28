using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Application.Features.Authors.Queries.GetAuthorById;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Authors;

public sealed class GetAuthorByIdQueryHandlerTests
{
    private readonly Mock<IAuthorQueryRepository> _authorQueryRepositoryMock;
    private readonly GetAuthorByIdQueryHandler _handler;

    public GetAuthorByIdQueryHandlerTests()
    {
        _authorQueryRepositoryMock = new Mock<IAuthorQueryRepository>();
        _handler = new GetAuthorByIdQueryHandler(_authorQueryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAuthorExists_ShouldReturnAuthorDto()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var query = new GetAuthorByIdQuery(authorId);
        var authorDto = new AuthorDto(authorId, "John", "Doe", []);

        _authorQueryRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authorDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(authorId);
        result.Value.FirstName.ShouldBe("John");
        result.Value.LastName.ShouldBe("Doe");
        result.Value.BookIds.ShouldNotBeNull();
        _authorQueryRepositoryMock.Verify(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAuthorDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var query = new GetAuthorByIdQuery(authorId);

        _authorQueryRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AuthorDto?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
    }
}