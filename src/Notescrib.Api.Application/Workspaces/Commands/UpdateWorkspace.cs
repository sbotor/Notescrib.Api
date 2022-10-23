using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class UpdateWorkspace
{
    public record Command(string Id, string Name, SharingDetails SharingDetails) : ICommand<Result>;

    internal class Handler : ICommandHandler<Command, Result>
    {
        private readonly IWorkspaceMapper _mapper;
        private readonly IWorkspaceRepository _repository;
        private readonly IPermissionService _permissionService;

        public Handler(IWorkspaceRepository repository, IPermissionService permissionService, IWorkspaceMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _permissionService = permissionService;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetWorkspaceByIdAsync(request.Id);
            if (workspace == null)
            {
                return Result<WorkspaceOverview>.NotFound();
            }

            if (!_permissionService.CanEdit(workspace))
            {
                return Result<WorkspaceOverview>.Forbidden();
            }

            workspace = _mapper.MapToEntity(request, workspace);
            await _repository.UpdateWorkspaceAsync(workspace);

            return Result.Success();
        }
    }
}
