using Ardalis.Result;
using Litrater.Application.Abstractions.Authentication;
using Litrater.Application.Abstractions.CQRS;
using Litrater.Application.Abstractions.Data;
using Litrater.Domain.Users;
using Litrater.Application.Features.Authentication.Dtos;

namespace Litrater.Application.Features.Authentication.Commands.Register;

internal sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : ICommandHandler<RegisterCommand, UserDto>
{
    public async Task<Result<UserDto>> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
        {
            return Result.Conflict("User with this email already exists");
        }

        var user = new User(
            Guid.NewGuid(),
            command.Email,
            passwordHasher.Hash(command.Password),
            command.FirstName,
            command.LastName);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.ToDto();
    }
}