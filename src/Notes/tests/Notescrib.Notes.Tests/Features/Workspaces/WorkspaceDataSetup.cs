using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;

namespace Notescrib.Notes.Tests.Features.Workspaces;

public static class WorkspaceDataSetup
{
    public static void SetupWorkspace(TestWorkspaceRepository repository)
        => repository.Items.Add(new()
        {
            Id = "1",
            OwnerId = "1",
            Name = "Workspace",
            Folders = new List<Folder>
            {
                new() { Name = "Folder 0", Children = new List<Folder> { new() { Name = "Folder 0.0" } } },
                new() { Name = "Folder 1" }
            }
        });
}
