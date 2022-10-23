using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Entities;

public class Folder : EntityIdBase, IOwnableShareable, ICreatedTimestamp, IUpdatedTimestamp
{
    public const string Separator = "/";

    public string Name { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public string? ParentPath { get; set; }
    public string WorkspaceId { get; set; } = null!;
    public SharingDetails SharingDetails { get; set; } = null!;

    public string AbsolutePath => string.IsNullOrWhiteSpace(ParentPath)
        ? Id!
        : string.Join(Separator, ParentPath, Id);

    public string[] Segments => AbsolutePath.Split(Separator);
    public bool IsRoot => string.IsNullOrWhiteSpace(ParentPath);

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
