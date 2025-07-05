namespace Litrater.Domain.Common;

public class AggregateRoot : Entity
{
    protected AggregateRoot(Guid? id = null) : base(id)
    {
    }
}