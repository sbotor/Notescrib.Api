using Notescrib.Notes.Api.Contracts;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Api.Models;

public class PagingRequest : IPagingRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = Paging.DefaultPageSize;

    public Paging GetPaging()
        => new(Page, PageSize);
}
