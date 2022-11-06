using MediatR;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class UpdateWorkspace
{
    public record Command(string Id, string Name, SharingInfo SharingInfo) : ICommand;

    internal class Handler : ICommandHandler<Command>
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

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetByIdAsync(request.Id);
            if (workspace == null)
            {
                throw new NotFoundException();
            }

            _sharingService.GuardCanEdit(workspace);

            workspace = _mapper.UpdateEntity(request, workspace);
            await _repository.UpdateAsync(workspace);

            return Unit.Value;
        }
    }
}
