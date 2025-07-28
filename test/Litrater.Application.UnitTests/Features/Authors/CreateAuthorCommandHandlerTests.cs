using Ardalis.Result;
using Litrater.Application.Abstractions.Data;
using Litrater.Application.Features.Authors.Commands.CreateAuthor;
using Litrater.Domain.Authors;
using Moq;
using Shouldly;

namespace Litrater.Application.UnitTests.Features.Authors;

public sealed class CreateAuthorCommandHandlerTests
{
    private readonly Mock<IAuthorCommandRepository> _authorCommandRepositoryMock;
    private readonly CreateAuthorCommandHandler _handler;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public CreateAuthorCommandHandlerTests()
    {
        _authorCommandRepositoryMock = new Mock<IAuthorCommandRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateAuthorCommandHandler(_authorCommandRepositoryMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_WhenAuthorDoesNotExist_ShouldCreateAuthorSuccessfully()
    {
        // Arrange
        var command = new CreateAuthorCommand("John", "Doe");

        _authorCommandRepositoryMock
            .Setup(x => x.ExistsByNameAsync(command.FirstName, command.LastName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.FirstName.ShouldBe(command.FirstName);
        result.Value.LastName.ShouldBe(command.LastName);
        result.Value.Id.ShouldNotBe(Guid.Empty);

        _authorCommandRepositoryMock.Verify(x => x.ExistsByNameAsync(command.FirstName, command.LastName, It.IsAny<CancellationToken>()), Times.Once);

        _authorCommandRepositoryMock.Verify(x => x.AddAsync(
            It.Is<Author>(a =>
                a.FirstName == command.FirstName &&
                a.LastName == command.LastName),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAuthorAlreadyExists_ShouldReturnConflictResult()
    {
        // Arrange
        var command = new CreateAuthorCommand("John", "Doe");

        _authorCommandRepositoryMock
            .Setup(x => x.ExistsByNameAsync(command.FirstName, command.LastName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Status.ShouldBe(ResultStatus.Conflict);

        _authorCommandRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}