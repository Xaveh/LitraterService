using Litrater.Domain.Common;

namespace Litrater.Domain.Users;

public sealed class User : AggregateRoot
{
    public Guid KeycloakUserId { get; private set; }

#pragma warning disable CS8618 // Required by Entity Framework
    private User() { }

    public User(Guid id, Guid keycloakUserId) : base(id)
    {
        KeycloakUserId = keycloakUserId;
    }
}