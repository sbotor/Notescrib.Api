namespace Notescrib.Notes.Application.Models;

public readonly struct Paging
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public int PageNumber { get; }
    public int PageSize { get; }

    public Paging(int? pageNumber = null, int? pageSize = null)
    {
        PageNumber = pageNumber ?? 1;
        PageSize = pageSize ?? DefaultPageSize;
    }
}
