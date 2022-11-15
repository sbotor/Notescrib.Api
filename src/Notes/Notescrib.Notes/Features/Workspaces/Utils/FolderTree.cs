using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Workspaces.Utils;

public class FolderTree : BfsTree<Folder>
{
    public FolderTree(IEnumerable<Folder> roots) : base(roots)
    {
    }
    
    public FolderTree()
    {
    }

    public TreeNode<Folder> Add(Folder item, string? parent)
    {
        if (this.Any(x => x.Name == item.Name))
        {
            throw new DuplicationException<Folder>();
        }
        
        if (!string.IsNullOrEmpty(parent))
        {
            var parentNode = AsNodeEnumerable().FirstOrDefault(x => x.Item.Name == parent);
            if (parentNode == null)
            {
                throw new NotFoundException<Folder>(parent);
            }
            
            var node = Add(item, parentNode);
            return node;
        }
        
        AddCore(Roots, item);
        return new(item, 0);
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
        if (Count >= Size.Counts.MaxFolders)
        {
            throw new AppException();
        }
        
        target.Add(item);
        Count++;
    }
}
