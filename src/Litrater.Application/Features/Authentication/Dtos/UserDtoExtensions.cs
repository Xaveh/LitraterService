using Litrater.Domain.Users;

namespace Litrater.Application.Features.Authentication.Dtos;

public static class UserDtoExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto(
            Id: user.Id,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            IsActive: user.IsActive,
            UserRole: user.UserRole.ToString()
        );
    }
}