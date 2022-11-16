using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class CreateNote
{
    public record Command(string Name, string WorkspaceId, string Folder, IReadOnlyCollection<string> Labels, SharingInfo SharingInfo)
        : ICommand<string>;

    internal class Handler : ICommandHandler<Command, string>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IUserContextProvider _userContext;

        public Handler(
            INoteRepository noteRepository,
            IWorkspaceRepository workspaceRepository,
            IPermissionGuard permissionGuard,
            IUserContextProvider userContext)
        {
            _noteRepository = noteRepository;
            _workspaceRepository = workspaceRepository;
            _permissionGuard = permissionGuard;
            _userContext = userContext;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var ownerId = _userContext.UserId;
            if (ownerId == null)
            {
                throw new AppException();
            }

            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(request.WorkspaceId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>(request.WorkspaceId);
            }
            
            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            if (!string.IsNullOrEmpty(request.Folder)
                && workspace.Folders.All(x => x.Name != request.Folder))
            {
                throw new NotFoundException<Folder>();
            }
            
            if (await _noteRepository.ExistsAsync(request.WorkspaceId, request.Folder, request.Name, cancellationToken))
            {
                throw new DuplicationException<Note>();
            }
            
            var note = new Note
            {
                Name = request.Name,
                OwnerId = ownerId,
                WorkspaceId = request.WorkspaceId,
                Folder = request.Folder,
                Contents = Array.Empty<NoteSection>(),
                SharingInfo = request.SharingInfo ?? new(),
                Labels = request.Labels.ToArray()
            };

            await _noteRepository.AddNote(note, cancellationToken);
            return note.Id;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.WorkspaceId)
                .NotEmpty();

            RuleFor(x => x.Folder)
                .NotNull();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Size.Name.Max);

            RuleFor(x => x.Labels.Count)
                .LessThanOrEqualTo(Size.Note.MaxLabelCount);
            RuleForEach(x => x.Labels)
                .NotEmpty();

            RuleFor(x => x.SharingInfo)
                .NotNull()
                .SetValidator(new SharingInfoValidator());
        }
    }
}
