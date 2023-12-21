namespace Notescrib.Notes.Contracts;

public interface ISortingProvider<in TSort> where TSort : struct, Enum
{
    public string GetSortName(TSort value);
}
