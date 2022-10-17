using Notescrib.Api.Application.Common.Services;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Queries;

public static class GetUserWorkspaces
{
    public record Query(IPaging Paging, ISorting Sorting) : IQuery<Result<PagedList<WorkspaceDetails>>>, IPagingRequest;

    internal class Handler : IQueryHandler<Query, Result<PagedList<WorkspaceDetails>>>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IUserContextService _userContextService;
        private readonly IWorkspaceMapper _mapper;

        public Handler(IWorkspaceRepository repository, IUserContextService userContextService, IWorkspaceMapper mapper)
        {
            _repository = repository;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<WorkspaceDetails>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var sorting = Sorting.GetDefaultIfEmpty(request.Sorting, nameof(Workspace.Name));

            var ownerId = _userContextService.UserId;
            if (ownerId == null)
            {
                return Result<PagedList<WorkspaceDetails>>.Failure();
            }

            var result = await _repository.GetUserWorkspacesAsync(ownerId, request.Paging, sorting);
            var response = result.Map(x => _mapper.MapToResponse(x));

            return Result<PagedList<WorkspaceDetails>>.Success(response);
        }
    }
}
