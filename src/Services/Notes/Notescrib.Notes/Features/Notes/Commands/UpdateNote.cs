using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class UpdateNote
{
    public record Command(
            string Id,
            string Name,
            string FolderId,
            IReadOnlyCollection<string> Labels,
            SharingInfo SharingInfo)
        : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(INoteRepository noteRepository, IWorkspaceRepository workspaceRepository, IPermissionGuard permissionGuard)
        {
            _noteRepository = noteRepository;
            _workspaceRepository = workspaceRepository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetNoteByIdAsync(request.Id, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException<Note>();
            }

            _permissionGuard.GuardCanEdit(note.OwnerId);
            
            if (await _noteRepository.ExistsAsync(note.WorkspaceId, request.FolderId, note.Name, cancellationToken))
            {
                throw new DuplicationException<Note>();
            }

            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(note.WorkspaceId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }

            if (new FolderTree(workspace.Folders).All(x => x.Id != request.FolderId))
            {
                throw new NotFoundException<Folder>();
            }

            note.Name = request.Name;
            note.FolderId = request.FolderId;
            note.Labels = request.Labels.ToArray();
            note.SharingInfo = request.SharingInfo;

            await _noteRepository.UpdateNoteAsync(note, cancellationToken);
            
            return Unit.Value;
        }
    }
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
            
            RuleFor(x => x.FolderId)
                .NotNull();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Size.Name.MaxLength);

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
