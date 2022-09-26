namespace Notescrib.Api.Core.Entities;

public class Workspace : IdEntityBase<string>
{
    public string Name { get; set; } = string.Empty;
    public ICollection<FolderPath> Folders { get; set; } = new List<FolderPath>();
    public SharingDetails SharingDetails { get; set; } = new();
    public string OwnerId { get; set; } = string.Empty;
}
