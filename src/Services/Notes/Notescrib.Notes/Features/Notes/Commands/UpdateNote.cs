using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Utils;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
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
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            INoteRepository noteRepository,
            IWorkspaceRepository workspaceRepository,
            IPermissionGuard permissionGuard,
            IDateTimeProvider dateTimeProvider)
        {
            _noteRepository = noteRepository;
            _workspaceRepository = workspaceRepository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetByIdAsync(request.Id, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException<Note>(request.Id);
            }

            _permissionGuard.GuardCanEdit(note.OwnerId);

            var workspace = await _workspaceRepository.GetByOwnerIdAsync(_permissionGuard.UserContext.UserId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }

            if (new FolderTree(workspace).All(x => x.Id != request.FolderId))
            {
                throw new NotFoundException<Folder>(request.FolderId);
            }

            note.Name = request.Name;
            note.FolderId = request.FolderId;
            note.Labels = request.Labels.ToArray();
            note.SharingInfo = request.SharingInfo;
            note.Updated = _dateTimeProvider.Now;

            await _noteRepository.UpdateAsync(note, cancellationToken);
            
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
                .MaximumLength(Counts.Name.MaxLength);

            RuleFor(x => x.Labels.Count)
                .LessThanOrEqualTo(Counts.Note.MaxLabelCount);
            RuleForEach(x => x.Labels)
                .NotEmpty();

            RuleFor(x => x.SharingInfo)
                .NotNull()
                .SetValidator(new SharingInfoValidator());
        }
    }
}
