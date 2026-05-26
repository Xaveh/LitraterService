using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Dtos;
using Litrater.Application.Features.Books.Queries.GetBookById;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IBookQueryRepository> _bookQueryRepositoryMock;
    private readonly GetBookByIdQueryHandler _handler;

    public GetBookByIdQueryHandlerTests()
    {
        _bookQueryRepositoryMock = new Mock<IBookQueryRepository>();
        _handler = new GetBookByIdQueryHandler(_bookQueryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookExists_ShouldReturnBookDto()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var query = new GetBookByIdQuery(bookId);
        var bookDto = new BookDto(bookId, "Test Book", "1234567890123", [Guid.NewGuid()], [], null);

        _bookQueryRepositoryMock
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(bookId);
        result.Value.Title.ShouldBe("Test Book");
        result.Value.Isbn.ShouldBe("1234567890123");
        result.Value.AuthorIds.ShouldContain(bookDto.AuthorIds.First());
        _bookQueryRepositoryMock.Verify(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var query = new GetBookByIdQuery(bookId);

        _bookQueryRepositoryMock
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BookDto?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
    }
}