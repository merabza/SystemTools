namespace DomPrimitives;

public interface IRepository
{
}

public interface IRepository<T> : IRepository where T : AggregateRoot
{
}