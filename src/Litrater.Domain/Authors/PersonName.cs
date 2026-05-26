using Litrater.Domain.Common;

namespace Litrater.Domain.Authors;

public sealed record PersonName : IValueObject
{
#pragma warning disable CS8618 // Required by Entity Framework
    private PersonName() { }
#pragma warning restore CS8618

    public PersonName(string firstName, string lastName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        if (firstName.Length > 100)
        {
            throw new ArgumentException("First name cannot exceed 100 characters", nameof(firstName));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        if (lastName.Length > 100)
        {
            throw new ArgumentException("Last name cannot exceed 100 characters", nameof(lastName));
        }

        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public override string ToString() => $"{FirstName} {LastName}";
}
