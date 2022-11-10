using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Utils;

public class TreeNode
{
    public Folder Item { get; }
    public int Level { get; }
    public bool CanNestChildren => Level < Size.NestingLevel.Max;

    internal TreeNode(Folder item, int level)
    {
        Item = item;
        Level = level;
    }

    internal TreeNode(Folder item, TreeNode? parent) : this(item, parent?.Level + 1 ?? 0)
    {
    }
}

public class TreeChildNode : TreeNode
{
    public TreeNode? Parent { get; }
    
    internal TreeChildNode(Folder item, TreeNode? parent) : base(item, parent)
    {
        Parent = parent;
    }
}
