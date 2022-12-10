using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Tests.Features.Workspaces;

public static class WorkspaceDataSetup
{
    public static void SetupWorkspace(TestWorkspaceRepository repository)
        => repository.Items.Add(new()
        {
            Id = "1",
            OwnerId = "1",
            FolderTree = Folder.CreateRoot(new List<Folder>
            {
                new()
                {
                    Id = "F0",
                    Name = "Folder 0",
                    Children = new List<Folder> { new() { Id = "F0.0", Name = "Folder 0.0" } }
                },
                new() { Id = "F1", Name = "Folder 1" }
            })
        });
}
