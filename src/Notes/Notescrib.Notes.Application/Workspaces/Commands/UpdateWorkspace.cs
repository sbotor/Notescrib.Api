using MediatR;
using Notescrib.Notes.Application.Common.Providers;
using Notescrib.Notes.Application.Workspaces.Mappers;
using Notescrib.Notes.Core.Entities;
using Notescrib.Notes.Core.Exceptions;

namespace Notescrib.Notes.Application.Workspaces.Commands;

public static class UpdateWorkspace
{
    public record Command(string Id, string Name, SharingInfo SharingInfo) : IRequest;

    internal class Handler : IRequestHandler<Command>
    {
        private readonly IWorkspaceMapper _mapper;
        private readonly IWorkspaceRepository _repository;
        private readonly ISharingGuard _sharingGuard;

        public Handler(IWorkspaceRepository repository, ISharingGuard sharingGuard, IWorkspaceMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _sharingGuard = sharingGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetByIdAsync(request.Id);
            if (workspace == null)
            {
                throw new NotFoundException();
            }

            _sharingGuard.GuardCanEdit(workspace);

            workspace = _mapper.UpdateEntity(request, workspace);
            await _repository.UpdateAsync(workspace);

            return Unit.Value;
        }
    }
}
