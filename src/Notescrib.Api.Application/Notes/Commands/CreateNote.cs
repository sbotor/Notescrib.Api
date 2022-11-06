using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Notes.Mappers;
using Notescrib.Api.Application.Workspaces;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;

namespace Notescrib.Api.Application.Notes.Commands;

public static class CreateNote
{
    public record Command(string Name, string FolderId, SharingInfo? SharingInfo, IEnumerable<string> Labels) : ICommand<string>;

    internal class Handler : ICommandHandler<Command, string>
    {
        private readonly ISharingService _sharingService;
        private readonly INoteRepository _noteRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly INoteMapper _mapper;

        public Handler(ISharingService sharingService, INoteRepository noteRepository, IFolderRepository folderRepository, INoteMapper mapper)
        {
            _sharingService = sharingService;
            _noteRepository = noteRepository;
            _folderRepository = folderRepository;
            _mapper = mapper;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.GetByIdAsync(request.FolderId);
            if (folder == null)
            {
                throw new NotFoundException();
            }

            _sharingService.GuardCanEdit(folder);

            var note = _mapper.MapToEntity(
                request,
                folder.OwnerId,
                request.SharingInfo ?? folder.SharingInfo);

            return (await _noteRepository.AddAsync(note)).Id;
        }
    }
}
