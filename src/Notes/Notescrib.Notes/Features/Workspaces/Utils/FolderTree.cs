using Notescrib.Notes.Models.Exceptions;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Utils;

public class FolderTree
{
    private readonly ICollection<Folder> _folders = new List<Folder>();

    private IReadOnlyCollection<Folder>? _readOnlyFolders;
    public IReadOnlyCollection<Folder> RootFolders => _readOnlyFolders ??= new ReadOnlyCollectionWrapper<Folder>(_folders);

    private int? _count;
    public int Count => _count ??= AsEnumerable().Count();

    public TreeChildNode Add(Folder item, string? parent)
    {
        if (AsEnumerable().Any(x => x.Item.Name == item.Name))
        {
            throw new AppException();
        }
        
        if (string.IsNullOrEmpty(parent))
        {
            var parentNode = AsEnumerable().FirstOrDefault(x => x.Item.Name == parent);
            if (parentNode == null)
            {
                throw new NotFoundException();
            }
            
            var node = Add(item, parentNode);
            return node;
        }
        else
        {
            AddCore(_folders, item);
            return new(item, null);
        }
    }

    private TreeChildNode Add(Folder item, TreeNode parent)
    {
        if (!parent.CanNestChildren)
        {
            throw new AppException();
        }
            
        parent.Item.Children.Add(item);
        return new(item, parent);
    }

    private IEnumerable<TreeNode> AsEnumerable()
    {
        var queue = new Queue<Folder>();
        foreach (var folder in _folders)
        {
            queue.Enqueue(folder);
        }

        var level = 0;

        while (queue.Count > 0)
        {
            var levelSize = queue.Count;
            while (levelSize > 0)
            {
                var node = queue.Dequeue();
                levelSize--;
            
                yield return new TreeNode(node, level);

                foreach (var child in node.Children)
                {
                    queue.Enqueue(child);
                }
            }

            level++;
        }
    }

    private void AddCore(ICollection<Folder> target, Folder item)
    {
        if (Count >= Size.Counts.MaxFolders)
        {
            throw new AppException();
        }
        
        target.Add(item);
        _count++;
    }
}
