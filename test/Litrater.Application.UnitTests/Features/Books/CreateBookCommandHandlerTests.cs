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
    private readonly Mock<IAuthorCommandRepository> _authorCommandRepositoryMock;
    private readonly Mock<IBookCommandRepository> _bookCommandRepositoryMock;
    private readonly CreateBookCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateBookCommandHandlerTests()
    {
        _bookCommandRepositoryMock = new Mock<IBookCommandRepository>();
        _authorCommandRepositoryMock = new Mock<IAuthorCommandRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateBookCommandHandler(_bookCommandRepositoryMock.Object, _authorCommandRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookDoesNotExistAndAuthorsExist_ShouldCreateBookSuccessfully()
    {
        // Arrange
        var authorIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new CreateBookCommand("Test Book", "1234567890123", authorIds);
        var authors = new List<Author> { new(authorIds[0], new PersonName("John", "Doe")), new(authorIds[1], new PersonName("Jane", "Smith")) };

        _bookCommandRepositoryMock
            .Setup(x => x.ExistsByIsbnAsync(command.Isbn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _authorCommandRepositoryMock
            .Setup(x => x.GetAuthorsByIdsAsync(command.AuthorIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Title.ShouldBe(command.Title);
        result.Value.Isbn.ShouldBe(command.Isbn);
        result.Value.AuthorIds.ShouldBe(authorIds);

        _bookCommandRepositoryMock.Verify(x => x.ExistsByIsbnAsync(command.Isbn, It.IsAny<CancellationToken>()), Times.Once);
        _authorCommandRepositoryMock.Verify(x => x.GetAuthorsByIdsAsync(command.AuthorIds, It.IsAny<CancellationToken>()),
            Times.Once);

        _bookCommandRepositoryMock.Verify(x => x.AddAsync(
            It.Is<Book>(b =>
                b.Title == command.Title &&
                b.Isbn.Value == command.Isbn &&
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

        _bookCommandRepositoryMock
            .Setup(x => x.ExistsByIsbnAsync(command.Isbn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Conflict);

        _authorCommandRepositoryMock.Verify(
            x => x.GetAuthorsByIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), Times.Never);
        _bookCommandRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenSomeAuthorIdsAreInvalid_ShouldReturnInvalidResult()
    {
        // Arrange
        var authorIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var command = new CreateBookCommand("Test Book", "1234567890123", authorIds);
        var authors = new List<Author> { new(authorIds[0], new PersonName("John", "Doe")) }; // Only one author found

        _bookCommandRepositoryMock
            .Setup(x => x.ExistsByIsbnAsync(command.Isbn, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _authorCommandRepositoryMock
            .Setup(x => x.GetAuthorsByIdsAsync(command.AuthorIds, It.IsAny<CancellationToken>()))
            .ReturnsAsync(authors);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e => e.Identifier == nameof(command.AuthorIds));
        _bookCommandRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Book>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}