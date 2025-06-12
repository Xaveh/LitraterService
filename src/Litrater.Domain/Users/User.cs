using Litrater.Domain.Common;

namespace Litrater.Domain.Users;

public sealed class User : AggregateRoot
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; }
    public UserRole UserRole { get; private set; }

#pragma warning disable CS8618 // Required by Entity Framework
    private User() { }

    public User(Guid id, string email, string passwordHash, string firstName, string lastName, UserRole userRole = UserRole.User) : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;
        UserRole = userRole;
    }
}
