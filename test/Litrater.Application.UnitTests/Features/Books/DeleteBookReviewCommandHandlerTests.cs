using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Commands.DeleteBookReview;
using Litrater.Domain.Books;
using Litrater.Domain.Users;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class DeleteBookReviewCommandHandlerTests
{
    private readonly Mock<IBookCommandRepository> _bookCommandRepositoryMock;
    private readonly DeleteBookReviewCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public DeleteBookReviewCommandHandlerTests()
    {
        _bookCommandRepositoryMock = new Mock<IBookCommandRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new DeleteBookReviewCommandHandler(_bookCommandRepositoryMock.Object, _userRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsOwner_ShouldDeleteBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var internalUserId = Guid.NewGuid();
        var keycloakUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Test content", new Rating(5), internalUserId)!;
        var command = new DeleteBookReviewCommand(review.Id, keycloakUserId);
        var user = new User(internalUserId, keycloakUserId);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _userRepositoryMock
            .Setup(x => x.GetByKeycloakUserIdAsync(keycloakUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        book.Reviews.ShouldBeEmpty();

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByKeycloakUserIdAsync(keycloakUserId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsAdmin_ShouldDeleteBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var originalInternalUserId = Guid.NewGuid();
        var adminKeycloakUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Test content", new Rating(5), originalInternalUserId)!;
        var command = new DeleteBookReviewCommand(review.Id, adminKeycloakUserId, IsAdmin: true);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        book.Reviews.ShouldBeEmpty();

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByKeycloakUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var reviewId = Guid.NewGuid();
        var keycloakUserId = Guid.NewGuid();
        var command = new DeleteBookReviewCommand(reviewId, keycloakUserId);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByKeycloakUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ShouldReturnUnauthorizedResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var internalUserId = Guid.NewGuid();
        var keycloakUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Test content", new Rating(5), internalUserId)!;
        var command = new DeleteBookReviewCommand(review.Id, keycloakUserId);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _userRepositoryMock
            .Setup(x => x.GetByKeycloakUserIdAsync(keycloakUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Unauthorized);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerAndNotAdmin_ShouldReturnForbiddenResult()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var originalInternalUserId = Guid.NewGuid();
        var differentKeycloakUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Test content", new Rating(5), originalInternalUserId)!;
        var command = new DeleteBookReviewCommand(review.Id, differentKeycloakUserId);
        var differentUser = new User(Guid.NewGuid(), differentKeycloakUserId);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        _userRepositoryMock
            .Setup(x => x.GetByKeycloakUserIdAsync(differentKeycloakUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(differentUser);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Forbidden);
        book.Reviews.Count.ShouldBe(1); // Review was not removed

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerButIsAdmin_ShouldDeleteAnyBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var originalInternalUserId = Guid.NewGuid();
        var adminKeycloakUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Another user's review", new Rating(3), originalInternalUserId)!;
        var command = new DeleteBookReviewCommand(review.Id, adminKeycloakUserId, IsAdmin: true);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        book.Reviews.ShouldBeEmpty();

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByKeycloakUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}