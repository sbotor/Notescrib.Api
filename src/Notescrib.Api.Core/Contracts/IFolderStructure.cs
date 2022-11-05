namespace Notescrib.Api.Core.Contracts;

public interface IFolderStructure : IEntityId
{
    public string? ParentId { get; }
}
