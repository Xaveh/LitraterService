using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Books.Commands.UpdateBookReview;
using Litrater.Domain.Books;
using Litrater.Domain.Users;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Books;

public sealed class UpdateBookReviewCommandHandlerTests
{
    private readonly Mock<IBookCommandRepository> _bookCommandRepositoryMock;
    private readonly UpdateBookReviewCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public UpdateBookReviewCommandHandlerTests()
    {
        _bookCommandRepositoryMock = new Mock<IBookCommandRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _handler = new UpdateBookReviewCommandHandler(_bookCommandRepositoryMock.Object, _userRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsOwner_ShouldUpdateBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var internalUserId = Guid.NewGuid();
        var keycloakUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Original content", new Rating(5), internalUserId)!;
        var command = new UpdateBookReviewCommand(review.Id, "Updated content", 4, keycloakUserId);
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
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(bookId);
        result.Value.UserId.ShouldBe(internalUserId);

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByKeycloakUserIdAsync(keycloakUserId, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenBookReviewExistsAndUserIsAdmin_ShouldUpdateBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var originalInternalUserId = Guid.NewGuid();
        var adminKeycloakUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Original content", new Rating(5), originalInternalUserId)!;
        var command = new UpdateBookReviewCommand(review.Id, "Admin updated content", 3, adminKeycloakUserId, IsAdmin: true);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(bookId);
        result.Value.UserId.ShouldBe(originalInternalUserId); // Original user ID should remain

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
        var command = new UpdateBookReviewCommand(reviewId, "Updated content", 4, keycloakUserId);

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
        var review = book.AddReview(Guid.NewGuid(), "Original content", new Rating(5), internalUserId)!;
        var command = new UpdateBookReviewCommand(review.Id, "Updated content", 4, keycloakUserId);

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
        var review = book.AddReview(Guid.NewGuid(), "Original content", new Rating(5), originalInternalUserId)!;
        var command = new UpdateBookReviewCommand(review.Id, "Unauthorized update", 4, differentKeycloakUserId);
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

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwnerButIsAdmin_ShouldUpdateBookReviewSuccessfully()
    {
        // Arrange
        var bookId = Guid.NewGuid();
        var originalInternalUserId = Guid.NewGuid();
        var adminKeycloakUserId = Guid.NewGuid();
        var book = new Book(bookId, "Test Book", new Isbn("1234567890123"), []);
        var review = book.AddReview(Guid.NewGuid(), "Original content", new Rating(5), originalInternalUserId)!;
        var command = new UpdateBookReviewCommand(review.Id, "Admin override update", 2, adminKeycloakUserId, IsAdmin: true);

        _bookCommandRepositoryMock
            .Setup(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(book);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Content.ShouldBe(command.Content);
        result.Value.Rating.ShouldBe(command.Rating);
        result.Value.BookId.ShouldBe(bookId);
        result.Value.UserId.ShouldBe(originalInternalUserId); // Original user ID should remain

        _bookCommandRepositoryMock.Verify(x => x.GetByReviewIdAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(x => x.GetByKeycloakUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}