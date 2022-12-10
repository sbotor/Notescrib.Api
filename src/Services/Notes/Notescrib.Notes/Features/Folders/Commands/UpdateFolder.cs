using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Folders.Commands;

public static class UpdateFolder
{
    public record Command(string FolderId, string Name) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IUserContextProvider _userContextProvider;

        public Handler(IWorkspaceRepository repository, IPermissionGuard permissionGuard, IDateTimeProvider dateTimeProvider, IUserContextProvider userContextProvider)
        {
            _repository = repository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
            _userContextProvider = userContextProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _userContextProvider.UserId;
            var workspace = await _repository.GetByOwnerIdAsync(userId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }
            
            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            var found = workspace.FolderTree.ToDfsEnumerable()
                .FirstOrDefault(x => x.Item.Id == request.FolderId)?.Item;
            if (found == null)
            {
                throw new NotFoundException<Folder>(request.FolderId);
            }

            found.Name = request.Name;

            var now = _dateTimeProvider.Now;
            found.Updated = now;

            await _repository.UpdateAsync(workspace, cancellationToken);
            
            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FolderId)
                .NotEmpty()
                .NotEqual(Folder.RootId);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Counts.Name.MaxLength);
        }
    }
}
