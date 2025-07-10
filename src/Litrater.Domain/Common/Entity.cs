namespace Litrater.Domain.Common;

public abstract class Entity
{
    protected Entity(Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
    }

    public Guid Id { get; }
    public DateTimeOffset CreatedDate { get; private set; }
    public DateTimeOffset? ModifiedDate { get; private set; }

    public void SetCreatedDate(DateTimeOffset createdDate)
    {
        CreatedDate = createdDate;
    }

    public void SetModifiedDate(DateTimeOffset modifiedDate)
    {
        ModifiedDate = modifiedDate;
    }
}