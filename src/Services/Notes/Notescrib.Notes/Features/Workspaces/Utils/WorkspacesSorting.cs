using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Features.Workspaces.Utils;

public enum WorkspacesSorting
{
    Name
}

internal class WorkspacesSortingProvider : SortingProvider<WorkspacesSorting>
{
    public override string GetSortName(WorkspacesSorting value)
        => value switch
        {
            WorkspacesSorting.Name => nameof(Workspace.Name),
            _ => base.GetSortName(value)
        };
}
