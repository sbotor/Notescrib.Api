using Notescrib.Notes.Contracts;
using Notescrib.Notes.Models.Exceptions;

namespace Notescrib.Notes.Models;

public abstract class SortingProvider<TSort> : ISortingProvider<TSort>
    where TSort : struct, Enum
{
    public virtual string GetSortName(TSort value) => throw new AppException($"Unrecognized sorting value '{value}'.");
}
