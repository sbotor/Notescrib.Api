namespace Notescrib.Api.Core.Entities;

public class EntityIdBase<TKey> where TKey : IEquatable<TKey>
{
    public TKey? Id { get; set; }
}

public class EntityIdBase : EntityIdBase<string>
{
}
