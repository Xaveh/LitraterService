using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Queries.GetBookById;
using Litrater.Domain.Authors;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class GetBookByIdQueryHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly GetBookByIdQueryHandler _handler;

    public GetBookByIdQueryHandlerTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _handler = new GetBookByIdQueryHandler(_bookRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookExists_ShouldReturnBookDto()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var query = new GetBookByIdQuery(bookId);
        var authors = new List<Author> { new(Guid.NewGuid(), "John", "Doe") };
        var book = new Book(bookId, "Test Book", "1234567890123", authors);

        _bookRepositoryMock
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Id.ShouldBe(bookId);
        result.Value.Title.ShouldBe("Test Book");
        result.Value.Isbn.ShouldBe("1234567890123");
        result.Value.AuthorIds.ShouldContain(authors[0].Id);
        _bookRepositoryMock.Verify(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var query = new GetBookByIdQuery(bookId);

        _bookRepositoryMock
            .Setup(x => x.GetByIdAsync(bookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
    }
}