using Notescrib.Api.Application.Workspaces.Queries;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Contracts.Workspaces;

public class GetUserWorkspacesRequest : PagingApiRequest
{
    public GetUserWorkspaces.Query ToQuery()
        => new(new Paging(PageNumber, PageSize));
}
