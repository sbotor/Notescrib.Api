using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Utils;

public class FolderTree
{
    public ICollection<Folder> Folders { get; set; } = new List<Folder>();

    private int? _count;
    public int Count => _count ??= AsEnumerable().Count();

    public TreeChildNode<Folder> Add(Folder item, string? parent)
    {
        if (AsEnumerable().Any(x => x.Item.Name == item.Name))
        {
            throw new DuplicationException<Folder>();
        }
        
        if (!string.IsNullOrEmpty(parent))
        {
            var parentNode = AsEnumerable().FirstOrDefault(x => x.Item.Name == parent);
            if (parentNode == null)
            {
                throw new NotFoundException<Folder>(parent);
            }
            
            var node = Add(item, parentNode);
            return node;
        }
        
        AddCore(Folders, item);
        return new(item, null);
    }

    public bool Exists(string? name)
        => name == null
           || AsEnumerable().Any(x => x.Item.Name == name);

    private TreeChildNode<Folder> Add(Folder item, TreeNode<Folder> parent)
    {
        if (!parent.CanNestChildren)
        {
            throw new AppException();
        }

        AddCore(parent.Item.Children, item);
        return new(item, parent);
    }

    private IEnumerable<TreeNode<Folder>> AsEnumerable()
        => new TreeEnumerable(Folders);
    
    private void AddCore(ICollection<Folder> target, Folder item)
    {
        if (Count >= Size.Counts.MaxFolders)
        {
            throw new AppException();
        }
        
        target.Add(item);
        _count++;
    }

    private class TreeEnumerable : TreeEnumerable<Folder>
    {
        public TreeEnumerable(IEnumerable<Folder> rootItems)
            : base(rootItems)
        {
        }

        protected override IEnumerable<Folder> GetChildren(Folder item) => item.Children;
    }
}
