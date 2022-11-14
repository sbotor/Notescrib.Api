﻿using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Utils;

public class TreeNode<T>
{
    public T Item { get; }
    public int Level { get; }
    public bool CanNestChildren => Level < Size.NestingLevel.Max;

    internal TreeNode(T item, int level)
    {
        Item = item;
        Level = level;
    }
}
