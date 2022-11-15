using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class CreateWorkspace
{
    public record Command(string Name) : ICommand<string>;

    internal class Handler : ICommandHandler<Command, string>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IUserContextProvider _userContext;

        public Handler(IWorkspaceRepository repository, IUserContextProvider userContext)
        {
            _repository = repository;
            _userContext = userContext;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var ownerId = _userContext.UserId;
            if (ownerId == null)
            {
                throw new AppException("No user context found.");
            }

            if (await _repository.ExistsAsync(request.Name, cancellationToken))
            {
                throw new DuplicationException<Workspace>();
            }
            
            var workspace = new Workspace
            {
                Name = request.Name,
                OwnerId = ownerId
            };
            await _repository.AddWorkspaceAsync(workspace, cancellationToken);

            return workspace.Id;
        }
    }
}
