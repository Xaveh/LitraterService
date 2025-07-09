using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authors.Commands.UpdateAuthor;
using Litrater.Application.Features.Authors.Dtos;
using Litrater.Domain.Authors;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Authors;

public class UpdateAuthorCommandHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateAuthorCommandHandler _handler;

    public UpdateAuthorCommandHandlerTests()
    {
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _bookRepositoryMock = new Mock<IBookRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateAuthorCommandHandler(_authorRepositoryMock.Object, _bookRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenAuthorDoesNotExist()
    {
        // Arrange
        var command = new UpdateAuthorCommand(
            Id: Guid.NewGuid(),
            FirstName: "John",
            LastName: "Doe",
            BookIds: [Guid.NewGuid()]
        );

        _authorRepositoryMock
            .Setup(x => x.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalid_WhenSomeBookIdsAreInvalid()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var bookId1 = Guid.NewGuid();
        var bookId2 = Guid.NewGuid();
        var author = new Author(authorId, "John", "Doe");
        var book1 = new Book(bookId1, "Book 1", "1234567890123", []);

        var command = new UpdateAuthorCommand(
            Id: authorId,
            FirstName: "John",
            LastName: "Doe",
            BookIds: [bookId1, bookId2]
        );

        _authorRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        _bookRepositoryMock
            .Setup(x => x.GetBooksByIdsAsync(command.BookIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync([book1]); // Only one book found, but two requested

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.Identifier == nameof(command.BookIds));
    }

    [Fact]
    public async Task Handle_ShouldUpdateAuthor_WhenCommandIsValid()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var bookId1 = Guid.NewGuid();
        var bookId2 = Guid.NewGuid();
        var author = new Author(authorId, "John", "Doe");
        var book1 = new Book(bookId1, "Book 1", "1234567890123", []);
        var book2 = new Book(bookId2, "Book 2", "1234567890124", []);

        var command = new UpdateAuthorCommand(
            Id: authorId,
            FirstName: "Jane",
            LastName: "Smith",
            BookIds: [bookId1, bookId2]
        );

        _authorRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        _bookRepositoryMock
            .Setup(x => x.GetBooksByIdsAsync(command.BookIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync([book1, book2]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Ok);
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeOfType<AuthorDto>();
        result.Value.FirstName.ShouldBe("Jane");
        result.Value.LastName.ShouldBe("Smith");
        result.Value.BookIds.ShouldBe([bookId1, bookId2]);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}