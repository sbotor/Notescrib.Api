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
            var parentNode = AsNodeEnumerable().FirstOrDefault(x => x.Item.Name == parentId);
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

    public void Move(string id, string? newParentId)
    {
        var found = FindWithParent(x => x.Id == id);
        if (found == null)
        {
            throw new NotFoundException<Folder>();
        }

        if (found.Parent?.Id == newParentId)
        {
            return;
        }
        
        if (newParentId == null)
        {
            Roots.Add(found.Item);
            return;
        }

        if (found.Item.EnumerateChildren().Any(x => x.Id == newParentId))
        {
            throw new AppException("Cannot move folder to its own child.");
        }
        
        var newParent = this.First(x => x.Id == newParentId);

        RemoveCore(found.Parent?.Children, found.Item, false);
        newParent.Children.Add(found.Item);
    }

    public TreeChildNode<Folder>? FindWithParent(Func<Folder, bool> predicate)
    {
        foreach (var folder in this)
        {
            var found = folder.Children.SingleOrDefault(predicate);
            if (found == null)
            {
                continue;
            }
            
            return new(found, folder);
        }

        return null;
    }

    public void Remove(TreeChildNode<Folder> folder)
        => RemoveCore(folder.Parent?.Children, folder.Item, true);

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
