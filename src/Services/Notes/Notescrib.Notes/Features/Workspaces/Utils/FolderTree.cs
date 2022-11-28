using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Workspaces.Utils;

public class FolderTree : BfsTree<Folder>
{
    public FolderTree(IEnumerable<Folder> roots) : base(roots)
    {
    }

    public TreeNode<Folder> Add(Folder item, string? parentId)
    {
        if (this.Any(x => x.Name == item.Name))
        {
            throw new DuplicationException<Folder>();
        }
        
        if (!string.IsNullOrEmpty(parentId))
        {
            var parentNode = AsNodeEnumerable().FirstOrDefault(x => x.Item.Id == parentId);
            if (parentNode == null)
            {
                throw new NotFoundException<Folder>(parentId);
            }
            
            var node = Add(item, parentNode);
            return node;
        }
        
        AddCore(Roots, item);
        return new(item, 0);
    }

    public void Move(TreeChildNode<Folder> node, string? newParentId)
    {
        if (newParentId == null)
        {
            if (IsDuplicated(Roots, node.Item))
            {
                throw new DuplicationException<Folder>();
            }

            Roots.Add(node.Item);
            return;
        }

        if (node.Item.EnumerateChildren().Any(x => x.Id == newParentId))
        {
            throw new AppException("Cannot move folder to its own child.");
        }

        var newParent = this.First(x => x.Id == newParentId);
        if (IsDuplicated(newParent.Children, node.Item))
        {
            throw new DuplicationException<Folder>();
        }

        RemoveCore(node.Parent?.Children, node.Item, false);
        newParent.Children.Add(node.Item);
    }

    public void Remove(TreeChildNode<Folder> folder)
        => RemoveCore(folder.Parent?.Children, folder.Item, true);

    private static bool IsDuplicated(IEnumerable<Folder> folders, Folder folder)
        => folders.FirstOrDefault(x => x.Name == folder.Name && x.Id != folder.Id) != null;

    private void RemoveCore(ICollection<Folder>? target, Folder folder, bool decrementCount)
    {
        (target ?? Roots).Remove(folder);

        if (decrementCount)
        {
            Count--;
        }
    }

    private TreeNode<Folder> Add(Folder item, TreeNode<Folder> parent)
    {
        if (!parent.CanNestChildren)
        {
            throw new AppException();
        }

        AddCore(parent.Item.Children, item);
        return new(item, parent.Level + 1);
    }

    private void AddCore(ICollection<Folder> target, Folder item)
    {
        if (Count >= Size.Folder.MaxCount)
        {
            throw new AppException();
        }
        
        target.Add(item);
        Count++;
    }
}
