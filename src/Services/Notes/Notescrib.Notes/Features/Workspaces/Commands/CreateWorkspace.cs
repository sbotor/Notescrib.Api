using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class CreateWorkspace
{
    public record Command : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IUserContextProvider _userContext;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(IWorkspaceRepository repository, IUserContextProvider userContext, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
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
            var workspace = new Workspace { OwnerId = userId, Created = now, FolderTree = Folder.CreateRoot(now) };
            await _repository.AddAsync(workspace, cancellationToken);

            return Unit.Value;
        }
    }
}
