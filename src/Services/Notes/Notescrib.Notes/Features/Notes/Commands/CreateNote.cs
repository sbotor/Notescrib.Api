﻿using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Utils;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class CreateNote
{
    public record Command(string Name, string FolderId, IReadOnlyCollection<string> Labels, SharingInfo SharingInfo)
        : ICommand<NoteOverview>;

    internal class Handler : ICommandHandler<Command, NoteOverview>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Note, NoteOverview> _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            INoteRepository noteRepository,
            IWorkspaceRepository workspaceRepository,
            IPermissionGuard permissionGuard,
            IMapper<Note, NoteOverview> mapper,
            IDateTimeProvider dateTimeProvider)
        {
            _noteRepository = noteRepository;
            _workspaceRepository = workspaceRepository;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<NoteOverview> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _permissionGuard.UserContext.UserId;
            var workspace = await _workspaceRepository.GetByOwnerIdAsync(userId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }
            
            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            if (new FolderTree(workspace).All(x => x.Id != request.FolderId))
            {
                throw new NotFoundException<Folder>(request.FolderId);
            }

            var note = new Note
            {
                Name = request.Name,
                OwnerId = userId,
                FolderId = request.FolderId,
                NoteSectionTree = Array.Empty<NoteSection>(),
                SharingInfo = request.SharingInfo,
                Labels = request.Labels.ToArray(),
                Created = _dateTimeProvider.Now
            };

            await _noteRepository.AddNote(note, cancellationToken);
            return _mapper.Map(note);
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
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
