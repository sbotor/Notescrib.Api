using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class UpdateWorkspace
{
    public record Command(string Id, string Name, SharingInfo SharingInfo) : ICommand<Result>;

    internal class Handler : ICommandHandler<Command, Result>
    {
        private readonly IWorkspaceMapper _mapper;
        private readonly IWorkspaceRepository _repository;
        private readonly ISharingService _sharingService;

        public Handler(IWorkspaceRepository repository, ISharingService sharingService, IWorkspaceMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _sharingService = sharingService;
        }

        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetByIdAsync(request.Id);
            if (workspace == null)
            {
                return Result<WorkspaceOverview>.NotFound();
            }

            if (!_sharingService.CanEdit(workspace))
            {
                return Result<WorkspaceOverview>.Forbidden();
            }

            workspace = _mapper.UpdateEntity(request, workspace);
            await _repository.UpdateAsync(workspace);

            return Result.Success();
        }
    }
}
