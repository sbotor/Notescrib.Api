using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Contracts;

public interface ISortingProvider<in TSort> where TSort : struct, Enum
{
    public string GetSortName(TSort value);
}
