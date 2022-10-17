using System.Diagnostics.CodeAnalysis;
using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Helpers;

public class WorkspacePathComparer : IEqualityComparer<IWorkspacePath>
{
    public bool Equals(IWorkspacePath? x, IWorkspacePath? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        return x?.WorkspaceId == y?.WorkspaceId
            && x?.ParentPath == y?.ParentPath;
    }

    public int GetHashCode([DisallowNull] IWorkspacePath obj)
        => HashCode.Combine(obj.WorkspaceId, obj.ParentPath);

    public bool StartsWith(IWorkspacePath? x, IWorkspacePath? y)
    {
        if (x == null && y == null)
        {
            return true;
        }

        return x?.WorkspaceId == y?.WorkspaceId
            && (x?.ParentPath?.StartsWith(y?.ParentPath ?? string.Empty) ?? true);
    }
}
