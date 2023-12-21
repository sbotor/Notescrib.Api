using Notescrib.Models;
using Notescrib.WebApi.Contracts;

namespace Notescrib.WebApi.Models;

public class PagingRequest : IPagingRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = Paging.DefaultPageSize;

    public Paging GetPaging()
        => new(Page, PageSize);
}
