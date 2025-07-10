using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authors.Commands.DeleteAuthor;
using Litrater.Domain.Authors;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Authors;

public sealed class DeleteAuthorCommandHandlerTests
{
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly DeleteAuthorCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public DeleteAuthorCommandHandlerTests()
    {
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new DeleteAuthorCommandHandler(_authorRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAuthorExists_ShouldDeleteAuthorSuccessfully()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var command = new DeleteAuthorCommand(authorId);
        var author = new Author(authorId, "John", "Doe");

        _authorRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        _authorRepositoryMock.Verify(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()), Times.Once);
        _authorRepositoryMock.Verify(x => x.Delete(author), Times.Once);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAuthorDoesNotExist_ShouldReturnNotFoundResult()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var command = new DeleteAuthorCommand(authorId);

        _authorRepositoryMock
            .Setup(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.NotFound);

        _authorRepositoryMock.Verify(x => x.GetByIdAsync(authorId, It.IsAny<CancellationToken>()), Times.Once);
        _authorRepositoryMock.Verify(x => x.Delete(It.IsAny<Author>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}