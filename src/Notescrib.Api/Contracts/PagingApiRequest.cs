using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Contracts;

public class PagingApiRequest : IPaging
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = Paging.DefaultPageSize;
}
