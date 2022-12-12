using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class CreateWorkspace
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IFolderRepository _folderRepository;
        private readonly IUserContextProvider _userContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            IWorkspaceRepository repository,
            IFolderRepository folderRepository,
            IUserContextProvider userContext,
            IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _folderRepository = folderRepository;
            _userContext = userContext;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _userContext.UserId;
            if (await _repository.GetByOwnerIdAsync(userId, cancellationToken) != null)
            {
                throw new DuplicationException<Workspace>();
            }

            var now = _dateTimeProvider.Now;
            var workspace = new Workspace { OwnerId = userId, Created = now };
            await _repository.AddAsync(workspace, cancellationToken);

            await _folderRepository.AddAsync(
                new()
                {
                    Id = workspace.Id,
                    OwnerId = workspace.OwnerId,
                    Name = "*root",
                    Created = workspace.Created,
                    WorkspaceId = workspace.Id
                },
                CancellationToken.None);

            return Unit.Value;
        }
    }
}
