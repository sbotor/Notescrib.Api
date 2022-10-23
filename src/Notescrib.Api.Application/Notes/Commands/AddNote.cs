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
    public record Command(string Name, string FolderId, SharingDetails SharingDetails) : ICommand<Result<string>>;

    internal class Handler : ICommandHandler<Command, Result<string>>
    {
        private readonly IPermissionService _permissionService;
        private readonly INoteRepository _noteRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IMapper _mapper;

        public Handler(IPermissionService permissionService, INoteRepository noteRepository, IFolderRepository folderRepository, IMapper mapper)
        {
            _permissionService = permissionService;
            _noteRepository = noteRepository;
            _folderRepository = folderRepository;
            _mapper = mapper;
        }

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.GetFolderByIdAsync(request.FolderId);
            if (folder == null)
            {
                return Result<string>.NotFound();
            }

            if (!_permissionService.CanEdit(folder))
            {
                return Result<string>.Forbidden();
            }

            var note = _mapper.Map<Note>(request);

            return Result<string>.Success(await _noteRepository.AddNoteAsync(note));
        }
    }
}
