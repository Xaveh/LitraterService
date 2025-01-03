namespace Litrater.Domain.Shared;

public abstract class Entity : IEquatable<Entity>
{
    protected Entity(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? ModifiedDate { get; private set; }

    public bool Equals(Entity? other)
    {
        if (other is null)
        {
            return false;
        }

        if (other.GetType() != GetType())
        {
            return false;
        }

        return Id == other.Id;
    }

    public void UpdateModifiedDate()
    {
        ModifiedDate = DateTime.UtcNow;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        if (obj is not Entity entity)
        {
            return false;
        }

        return Id == entity.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        return left is not null && right is not null && left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}