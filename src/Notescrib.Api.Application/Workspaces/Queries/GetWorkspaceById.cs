using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Queries;

public static class GetWorkspaceById
{
    public record Query(string Id) : IQuery<Result<WorkspaceDetails>>;

    internal class Handler : IQueryHandler<Query, Result<WorkspaceDetails>>
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkspaceRepository _repository;
        private readonly IWorkspaceMapper _mapper;

        public Handler(IPermissionService permissionService, IWorkspaceRepository repository, IWorkspaceMapper mapper)
        {
            _permissionService = permissionService;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<WorkspaceDetails>> Handle(Query request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetWorkspaceByIdAsync(request.Id);
            if (workspace == null)
            {
                return Result<WorkspaceDetails>.NotFound();
            }

            if (!_permissionService.CanView(workspace))
            {
                return Result<WorkspaceDetails>.Forbidden();
            }

            return Result<WorkspaceDetails>.Success(_mapper.MapToResponse(workspace));
        }
    }
}
