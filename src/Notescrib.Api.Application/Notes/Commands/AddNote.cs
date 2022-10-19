using AutoMapper;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Notes.Commands;

public static class AddNote
{
    public record Command(
        string Name,
        string WorkspaceId,
        string? ParentPath,
        SharingDetails SharingDetails)
        : ICommand<Result<NoteOverview>>;

    internal class Handler : ICommandHandler<Command, Result<NoteOverview>>
    {
        private readonly IPermissionService _permissionService;
        private readonly IUserContextService _userContextService;
        private readonly INoteRepository _noteRepository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IMapper _mapper;

        public Handler(
            IPermissionService permissionService,
            IUserContextService userContextService,
            INoteRepository noteRepository,
            IWorkspaceRepository workspaceRepository,
            IMapper mapper)
        {
            _permissionService = permissionService;
            _userContextService = userContextService;
            _noteRepository = noteRepository;
            _workspaceRepository = workspaceRepository;
            _mapper = mapper;
        }

        public async Task<Result<NoteOverview>> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _workspaceRepository.GetWorkspaceByIdAsync(request.WorkspaceId);
            if (workspace == null)
            {
                return Result<NoteOverview>.NotFound($"Workspace (ID: {request.WorkspaceId}) not found.");
            }

            if (!_permissionService.CanEdit(workspace))
            {
                return Result<NoteOverview>.NotFound();
            }

            var note = _mapper.Map<Note>(request);
            note = await _noteRepository.AddNoteAsync(note);

            return Result<NoteOverview>.Success(_mapper.Map<NoteOverview>(note));
        }
    }
}
