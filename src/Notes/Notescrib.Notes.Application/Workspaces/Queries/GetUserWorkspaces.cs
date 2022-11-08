using MediatR;
using Notescrib.Notes.Application.Common.Contracts;
using Notescrib.Notes.Application.Common.Providers;
using Notescrib.Notes.Application.Workspaces.Mappers;
using Notescrib.Notes.Application.Workspaces.Models;
using Notescrib.Notes.Core.Contracts;
using Notescrib.Notes.Core.Exceptions;

namespace Notescrib.Notes.Application.Workspaces.Queries;

public static class GetUserWorkspaces
{
    public record Query(IPaging Paging, ISorting Sorting) : IPagingRequest<IPagedList<WorkspaceOverview>>;

    internal class Handler : IRequestHandler<Query, IPagedList<WorkspaceOverview>>
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

            return result.Map(_mapper.MapToOverview);
        }
    }
}
