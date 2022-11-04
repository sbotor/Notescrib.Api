using AutoMapper;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Notes.Commands;

public static class AddNote
{
    public record Command(string Name, string FolderId, SharingInfo SharingInfo) : ICommand<Result<string>>;

    internal class Handler : ICommandHandler<Command, Result<string>>
    {
        private readonly ISharingService _sharingService;
        private readonly INoteRepository _noteRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IMapper _mapper;

        public Handler(ISharingService sharingService, INoteRepository noteRepository, IFolderRepository folderRepository, IMapper mapper)
        {
            _sharingService = sharingService;
            _noteRepository = noteRepository;
            _folderRepository = folderRepository;
            _mapper = mapper;
        }

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.GetByIdAsync(request.FolderId);
            if (folder == null)
            {
                return Result<string>.NotFound();
            }

            if (!_sharingService.CanEdit(folder))
            {
                return Result<string>.Forbidden();
            }

            var note = _mapper.Map<Note>(request);

            return Result<string>.Success(await _noteRepository.AddAsync(note));
        }
    }
}
