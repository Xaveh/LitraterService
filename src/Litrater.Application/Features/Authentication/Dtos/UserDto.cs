namespace Litrater.Application.Features.Authentication.Dtos;

public record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    string UserRole
);