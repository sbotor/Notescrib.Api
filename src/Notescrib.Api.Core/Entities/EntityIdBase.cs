namespace Notescrib.Api.Core.Entities;

public class EntityIdBase<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
}

public class EntityIdBase : EntityIdBase<string>
{
}
