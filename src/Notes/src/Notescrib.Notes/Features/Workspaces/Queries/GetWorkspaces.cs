using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Workspaces.Models;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Queries;

public static class GetWorkspaces
{
    public record Query(Paging Paging, Sorting<WorkspacesSorting> Sorting) : IPagingSortingQuery<WorkspaceOverview, WorkspacesSorting>;

    internal class Handler : IQueryHandler<Query, PagedList<WorkspaceOverview>>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IUserContextProvider _userContext;
        private readonly IMapper<Workspace, WorkspaceOverview> _mapper;
        private readonly ISortingProvider<WorkspacesSorting> _sortingProvider;

        public Handler(
            IWorkspaceRepository repository,
            IUserContextProvider userContext,
            IMapper<Workspace, WorkspaceOverview> mapper,
            ISortingProvider<WorkspacesSorting> sortingProvider)
        {
            _repository = repository;
            _userContext = userContext;
            _mapper = mapper;
            _sortingProvider = sortingProvider;
        }

        public async Task<PagedList<WorkspaceOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var ownerId = _userContext.UserId;
            if (ownerId == null)
            {
                throw new AppException("No user context found.");
            }

            var result = await _repository.GetWorkspacesByOwnerIdAsync(ownerId,
                new(request.Paging, request.Sorting, _sortingProvider),
                cancellationToken);

            return result.Map(_mapper.Map);
        }
    }
}
