using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Core.Models;

public class Paging : IPaging
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = DefaultPageSize;

    public Paging(int? pageNumber = null, int? pageSize = null)
    {
        PageNumber = pageNumber ?? 1;
        PageSize = pageSize ?? DefaultPageSize;
    }
}
