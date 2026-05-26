using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Commands.CreateBookReview;
using Litrater.Domain.Books;
using Litrater.Domain.Users;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class CreateBookReviewCommandHandlerTests
{
    private readonly Mock<IBookCommandRepository> _bookCommandRepositoryMock;
    private readonly CreateBookReviewCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public CreateBookReviewCommandHandlerTests()
    {
        _bookCommandRepositoryMock = new Mock<IBookCommandRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new CreateBookReviewCommandHandler(_bookCommandRepositoryMock.Object, _userRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookExistsAndUserHasNotReviewed_ShouldCreateBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var internalUserId = Guid.NewGuid();
        var keycloakUserId = Guid.NewGuid();
        var command = new CreateBookReviewCommand("Great book!", 5, bookId, keycloakUserId);
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var user = new User(internalUserId, keycloakUserId);

        _userRepositoryMock
            .Setup(x => x.GetByKeycloakUserIdAsync(keycloakUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(command.BookId);
        result.Value.UserId.ShouldBe(internalUserId);
        book.Reviews.Count.ShouldBe(1);

        _userRepositoryMock.Verify(x => x.GetByKeycloakUserIdAsync(keycloakUserId, It.IsAny<CancellationToken>()), Times.Once);
        _bookCommandRepositoryMock.Verify(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUnauthorizedResult()
    {
        // Arrange
        var keycloakUserId = Guid.NewGuid();
        var command = new CreateBookReviewCommand("Great book!", 5, Guid.NewGuid(), keycloakUserId);

        _userRepositoryMock
            .Setup(x => x.GetByKeycloakUserIdAsync(keycloakUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Unauthorized);

        _userRepositoryMock.Verify(x => x.GetByKeycloakUserIdAsync(keycloakUserId, It.IsAny<CancellationToken>()), Times.Once);
        _bookCommandRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenBookDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var keycloakUserId = Guid.NewGuid();
        var command = new CreateBookReviewCommand("Great book!", 5, bookId, keycloakUserId);
        var user = new User(Guid.NewGuid(), keycloakUserId);

        _userRepositoryMock
            .Setup(x => x.GetByKeycloakUserIdAsync(keycloakUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserHasAlreadyReviewedBook_ShouldReturnConflictResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var internalUserId = Guid.NewGuid();
        var keycloakUserId = Guid.NewGuid();
        var command = new CreateBookReviewCommand("Great book!", 5, bookId, keycloakUserId);
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        book.AddReview(Guid.NewGuid(), "First review", new Rating(3), internalUserId);
        var user = new User(internalUserId, keycloakUserId);

        _userRepositoryMock
            .Setup(x => x.GetByKeycloakUserIdAsync(keycloakUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByIdAsync(command.BookId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Conflict);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}