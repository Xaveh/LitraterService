using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Commands.CreateBook;
using Litrater.Domain.Authors;
using Litrater.Domain.Books;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class CreateBookCommandHandlerTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateBookCommandHandler _handler;

    public CreateBookCommandHandlerTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateBookCommandHandler(_bookRepositoryMock.Object, _authorRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookDoesNotExistAndAuthorsExist_ShouldCreateBookSuccessfully()
    {
        // Arrange
        var authorIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new CreateBookCommand("Test Book", "1234567890123", authorIds);
        var authors = new List<Author> { new(authorIds[0], "John", "Doe"), new(authorIds[1], "Jane", "Smith") };

        _bookRepositoryMock
            .Setup(x => x.ExistsByIsbnAsync(command.Isbn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _authorRepositoryMock
            .Setup(x => x.GetAuthorsByIdsAsync(command.AuthorIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Title.ShouldBe(command.Title);
        result.Value.Isbn.ShouldBe(command.Isbn);
        result.Value.AuthorIds.ShouldBe(authorIds);

        _bookRepositoryMock.Verify(x => x.ExistsByIsbnAsync(command.Isbn, It.IsAny<CancellationToken>()), Times.Once);
        _authorRepositoryMock.Verify(x => x.GetAuthorsByIdsAsync(command.AuthorIds, It.IsAny<CancellationToken>()),
            Times.Once);

        _bookRepositoryMock.Verify(x => x.AddAsync(
            It.Is<Book>(b =>
                b.Title == command.Title &&
                b.Isbn == command.Isbn &&
                b.Authors.Count == authors.Count),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookAlreadyExists_ShouldReturnConflictResult()
    {
        // Arrange
        var authorIds = new List<Guid> { Guid.NewGuid() };
        var command = new CreateBookCommand("Test Book", "1234567890123", authorIds);

        _bookRepositoryMock
            .Setup(x => x.ExistsByIsbnAsync(command.Isbn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Conflict);

        _authorRepositoryMock.Verify(
            x => x.GetAuthorsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        _bookRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSomeAuthorIdsAreInvalid_ShouldReturnInvalidResult()
    {
        // Arrange
        var authorIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new CreateBookCommand("Test Book", "1234567890123", authorIds);
        var authors = new List<Author> { new(authorIds[0], "John", "Doe") }; // Only one author found

        _bookRepositoryMock
            .Setup(x => x.ExistsByIsbnAsync(command.Isbn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _authorRepositoryMock
            .Setup(x => x.GetAuthorsByIdsAsync(command.AuthorIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.Identifier == nameof(command.AuthorIds));
        _bookRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}