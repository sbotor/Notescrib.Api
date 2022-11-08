namespace Notescrib.Notes.Core.Contracts;

public interface IFolderStructure : IEntityId
{
    public string? ParentId { get; }
}
