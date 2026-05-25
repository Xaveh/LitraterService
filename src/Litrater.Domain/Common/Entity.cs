namespace Litrater.Domain.Common;

public abstract class Entity(Guid? id = null)
{
    public Guid Id { get; } = id ?? Guid.NewGuid();
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