using MediatR;
using Notescrib.Notes.Application.Common.Providers;
using Notescrib.Notes.Application.Notes.Mappers;
using Notescrib.Notes.Application.Workspaces;
using Notescrib.Notes.Core.Entities;
using Notescrib.Notes.Core.Exceptions;

namespace Notescrib.Notes.Application.Notes.Commands;

public static class CreateNote
{
    public record Command(string Name, string FolderId, SharingInfo? SharingInfo, IEnumerable<string> Labels) : IRequest<string>;

    internal class Handler : IRequestHandler<Command, string>
    {
        private readonly ISharingGuard _sharingGuard;
        private readonly INoteRepository _noteRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly INoteMapper _mapper;

        public Handler(ISharingGuard sharingGuard, INoteRepository noteRepository, IFolderRepository folderRepository, INoteMapper mapper)
        {
            _sharingGuard = sharingGuard;
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

            _sharingGuard.GuardCanEdit(folder);

            var note = _mapper.MapToEntity(
                request,
                folder.OwnerId,
                request.SharingInfo ?? folder.SharingInfo);

            return (await _noteRepository.AddAsync(note)).Id;
        }
    }
}
