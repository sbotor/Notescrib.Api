namespace Notescrib.Api.Core.Contracts;

public interface IEntityId<T> where T : IEquatable<T>
{
    public T Id { get; }
}

public interface IEntityId : IEntityId<string>
{
}
