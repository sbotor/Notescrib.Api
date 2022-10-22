using Notescrib.Api.Application.Workspaces.Queries;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Contracts.Workspaces;

public class GetUserWorkspacesRequest : PagingSortingApiRequest
{
    public GetUserWorkspaces.Query ToQuery()
        => new(this,
            Sorting.Parse<Workspace>(this, nameof(Workspace.Name)));
}
