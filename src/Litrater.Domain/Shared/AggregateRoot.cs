namespace Litrater.Domain.Shared;

public class AggregateRoot : Entity
{
    protected AggregateRoot(Guid? id = null) : base(id)
    {
    }
}