namespace Litrater.Domain.Common;

public abstract class Entity
{
    protected Entity(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        CreatedDate = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public DateTime CreatedDate { get; }
    public DateTime? ModifiedDate { get; }
}