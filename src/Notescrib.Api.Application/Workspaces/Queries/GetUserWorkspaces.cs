using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Queries;

public static class GetUserWorkspaces
{
    public record Query(IPaging Paging, ISorting Sorting) : IQuery<Result<IPagedList<WorkspaceOverview>>>, IPagingRequest;

    internal class Handler : IQueryHandler<Query, Result<IPagedList<WorkspaceOverview>>>
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

        public async Task<Result<IPagedList<WorkspaceOverview>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var ownerId = _userContextService.UserId;
            if (ownerId == null)
            {
                return Result<IPagedList<WorkspaceOverview>>.Failure();
            }

            var result = await _repository.GetUserWorkspacesAsync(ownerId, request.Paging, request.Sorting);
            var response = result.Map(x => _mapper.Map<WorkspaceOverview>(x));

            return Result<IPagedList<WorkspaceOverview>>.Success(response);
        }
    }
}
