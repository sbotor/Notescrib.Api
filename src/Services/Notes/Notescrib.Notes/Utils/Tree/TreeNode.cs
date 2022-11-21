namespace Notescrib.Notes.Utils.Tree;

public record TreeNode<T>(T Item, int Level)
{
    public bool CanNestChildren => Level < Size.NestingLevel.Max;
}
