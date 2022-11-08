namespace Notescrib.Notes.Core.Contracts;

public interface IPagedList<T>
{
    IList<T> Data { get; }
    int PageNumber { get; }
    int PageSize { get; }
    int TotalCount { get; }
    int TotalPageCount { get; }

    IPagedList<TOut> Map<TOut>(Func<T, TOut> mappingFunction);
}
