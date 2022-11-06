using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Queries;

public static class GetUserWorkspaces
{
    public record Query(IPaging Paging, ISorting Sorting) : IQuery<IPagedList<WorkspaceOverview>>, IPagingRequest;

    internal class Handler : IQueryHandler<Query, IPagedList<WorkspaceOverview>>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IUserContextProvider _userContext;
        private readonly IWorkspaceMapper _mapper;

        public Handler(IWorkspaceRepository repository, IUserContextProvider userContext, IWorkspaceMapper mapper)
        {
            _repository = repository;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<IPagedList<WorkspaceOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var ownerId = _userContext.UserId;
            if (ownerId == null)
            {
                throw new AppException("No user context found.");
            }

            var result = await _repository.GetUserWorkspacesAsync(ownerId, request.Paging, request.Sorting);

            return result.Map(_mapper.Map<WorkspaceOverview>);
        }
    }
}
