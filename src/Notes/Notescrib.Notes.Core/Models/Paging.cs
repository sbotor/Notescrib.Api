using Notescrib.Notes.Core.Contracts;

namespace Notescrib.Notes.Core.Models;

public class Paging : IPaging
{
    private const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public int PageNumber { get; set; }
    public int PageSize { get; set; }

    public Paging(int? pageNumber = null, int? pageSize = null)
    {
        PageNumber = pageNumber ?? 1;
        PageSize = pageSize ?? DefaultPageSize;
    }
}
