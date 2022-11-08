using Notescrib.Notes.Api.Models;
using Notescrib.Notes.Features.Workspaces.Queries;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Api.Features.Workspaces.Models;

public class GetWorkspacesRequest : PagingSortingRequest
{
    public GetWorkspaces.Query ToQuery()
        => new(new(PageNumber, PageSize), new(OrderBy, SortingDirection, SortingDefaults.Workspaces));
}
