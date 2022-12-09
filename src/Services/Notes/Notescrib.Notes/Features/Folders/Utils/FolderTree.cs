using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Folders.Utils;

public class FolderTree : BfsTree<Folder>
{
    public FolderTree(Workspace workspace) : base(workspace.FolderTree)
    {
    }

    public TreeNode<Folder> Add(Folder item, string parentId)
    {
        var parentNode = AsNodeEnumerable().FirstOrDefault(x => x.Item.Id == parentId);
        if (parentNode == null)
        {
            throw new NotFoundException<Folder>(parentId);
        }
            
        if (!parentNode.CanNestChildren)
        {
            throw new AppException("The parent folder cannot nest children.");
        }

        AddCore(parentNode.Item.Children, item);
        return new(item, parentNode.Level + 1);
    }

    public void Move(TreeChildNode<Folder> node, string newParentId)
    {
        if (node.Item.EnumerateChildren().Any(x => x.Id == newParentId))
        {
            throw new AppException("Cannot move folder to its own child.");
        }

        var newParent = this.First(x => x.Id == newParentId);

        RemoveCore(node.Parent?.Children, node.Item, false);
        newParent.Children.Add(node.Item);
    }

    public void Remove(TreeChildNode<Folder> folder)
        => RemoveCore(folder.Parent.Children, folder.Item, true);

    private void RemoveCore(ICollection<Folder>? target, Folder folder, bool decrementCount)
    {
        (target ?? Root.Children).Remove(folder);

        if (decrementCount)
        {
            Count--;
        }
    }

    private void AddCore(ICollection<Folder> target, Folder item)
    {
        if (Count >= Notescrib.Notes.Utils.Counts.Folder.MaxCount)
        {
            throw new AppException("Cannot add more folders.");
        }
        
        target.Add(item);
        Count++;
    }
}
