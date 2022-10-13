namespace Notescrib.Api.Core.Models;

public class Paging
{
    public const int DefaultPageSize = 10;

    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = DefaultPageSize;
}
