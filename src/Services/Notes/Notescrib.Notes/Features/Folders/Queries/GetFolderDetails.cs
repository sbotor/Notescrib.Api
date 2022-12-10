using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Folders.Queries;

public static class GetFolderDetails
{
    public record Query(string Id) : IQuery<FolderDetails>;

    internal class Handler : IQueryHandler<Query, FolderDetails>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly INoteRepository _noteRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Folder, FolderDetails> _folderMapper;
        private readonly IMapper<Note, NoteOverview> _noteMapper;

        public Handler(IWorkspaceRepository workspaceRepository, INoteRepository noteRepository,
            IPermissionGuard permissionGuard, IMapper<Folder, FolderDetails> folderMapper,
            IMapper<Note, NoteOverview> noteMapper)
        {
            _workspaceRepository = workspaceRepository;
            _noteRepository = noteRepository;
            _permissionGuard = permissionGuard;
            _folderMapper = folderMapper;
            _noteMapper = noteMapper;
        }

        public async Task<FolderDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = _permissionGuard.UserContext.UserId;
            var workspace = await _workspaceRepository.GetByOwnerIdAsync(userId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }
            
            var folder = new Tree<Folder>(workspace.FolderTree).EnumerateBreadthFirst()
                .FirstOrDefault(x => x.Item.Id == request.Id)?.Item;
            if (folder == null)
            {
                throw new NotFoundException<Folder>(request.Id);
            }

            var notes = await _noteRepository.GetAsync(folder.Id, _permissionGuard, cancellationToken);
            
            var details = _folderMapper.Map(folder);
            details.Notes = notes.Select(_noteMapper.Map).ToArray();

            return details;
        }
    }
}
