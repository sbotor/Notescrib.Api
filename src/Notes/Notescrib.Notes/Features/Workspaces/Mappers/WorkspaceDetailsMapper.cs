using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Workspaces.Models;

namespace Notescrib.Notes.Features.Workspaces.Mappers;

public class WorkspaceDetailsMapper : IMapper<Workspace, WorkspaceDetails>
{
    public WorkspaceDetails Map(Workspace item)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            OwnerId = item.OwnerId,
            FolderTree = MapFolders(item.FolderTree.Folders)
        };

    private static IReadOnlyCollection<FolderOverview> MapFolders(IEnumerable<Folder> source)
    {
        var stack = new Stack<Folder>();
        var mappedStack = new Stack<FolderOverview>();
        var output = new List<FolderOverview>();

        foreach (var folder in source)
        {
            var mapped = MapFolderOverview(folder);
            output.Add(mapped);

            stack.Push(folder);
            mappedStack.Push(mapped);
        }

        while (stack.Count > 0)
        {
            var original = stack.Pop();
            var mapped = mappedStack.Pop();

            foreach (var child in original.Children)
            {
                var mappedChild = MapFolderOverview(child);
                mapped.Children.Add(mappedChild);

                stack.Push(child);
                mappedStack.Push(mappedChild);
            }
        }

        return output;
    }

    private static FolderOverview MapFolderOverview(Folder item)
        => new() { Name = item.Name, Children = new List<FolderOverview>() };
}
