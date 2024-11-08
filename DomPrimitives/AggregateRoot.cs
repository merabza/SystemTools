namespace DomPrimitives;

public abstract class AggregateRoot : Entity
{
    // ReSharper disable once ConvertToPrimaryConstructor
    protected AggregateRoot(int id) : base(id)
    {
    }
}