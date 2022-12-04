using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
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
    public record Command(string Name, string WorkspaceId, string FolderId, IReadOnlyCollection<string> Labels, SharingInfo SharingInfo)
        : ICommand<NoteOverview>;

    internal class Handler : ICommandHandler<Command, NoteOverview>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IUserContextProvider _userContext;
        private readonly IMapper<Note, NoteOverview> _mapper;

        public Handler(
            INoteRepository noteRepository,
            IWorkspaceRepository workspaceRepository,
            IPermissionGuard permissionGuard,
            IUserContextProvider userContext,
            IMapper<Note, NoteOverview> mapper)
        {
            _noteRepository = noteRepository;
            _workspaceRepository = workspaceRepository;
            _permissionGuard = permissionGuard;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<NoteOverview> Handle(Command request, CancellationToken cancellationToken)
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

            if (!string.IsNullOrEmpty(request.FolderId)
                && workspace.Folders.All(x => x.Id != request.FolderId))
            {
                throw new NotFoundException<Folder>(request.FolderId);
            }
            
            if (await _noteRepository.ExistsAsync(request.WorkspaceId, request.FolderId, request.Name, cancellationToken))
            {
                throw new DuplicationException<Note>();
            }
            
            var note = new Note
            {
                Name = request.Name,
                OwnerId = ownerId,
                WorkspaceId = request.WorkspaceId,
                FolderId = request.FolderId,
                Contents = Array.Empty<NoteSection>(),
                SharingInfo = request.SharingInfo,
                Labels = request.Labels.ToArray()
            };

            await _noteRepository.AddNote(note, cancellationToken);
            return _mapper.Map(note);
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.WorkspaceId)
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
