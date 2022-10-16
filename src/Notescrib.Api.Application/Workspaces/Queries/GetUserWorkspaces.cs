using Notescrib.Api.Application.Common.Services;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Queries;

public static class GetUserWorkspaces
{
    public record Query(IPaging Paging) : IQuery<Result<PagedList<WorkspaceResponse>>>, IPagingRequest;

    internal class Handler : IQueryHandler<Query, Result<PagedList<WorkspaceResponse>>>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IUserContextService _userContextService;
        private readonly WorkspaceMapper _mapper;

        public Handler(IWorkspaceRepository repository, IUserContextService userContextService)
        {
            _repository = repository;
            _userContextService = userContextService;
            _mapper = new();
        }

        public async Task<Result<PagedList<WorkspaceResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var ownerId = _userContextService.UserId;
            if (ownerId == null)
            {
                return Result<PagedList<WorkspaceResponse>>.Failure();
            }

            var result = await _repository.GetUserWorkspacesAsync(ownerId, request.Paging.PageNumber, request.Paging.PageSize);
            var response = result.Map(x => _mapper.MapToResponse(x));

            return Result<PagedList<WorkspaceResponse>>.Success(response);
        }
    }
}
