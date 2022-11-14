using MediatR;
using MongoDB.Driver;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class CreateNote
{
    public record Command(string Name, string WorkspaceId, string? Folder, SharingInfo? SharingInfo)
        : IRequest<string>;

    internal class Handler : IRequestHandler<Command, string>
    {
        private readonly IMongoCollection<Note> _collection;
        private readonly IMongoCollection<Workspace> _workspaceCollection;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IUserContextProvider _userContext;

        public Handler(
            IMongoCollection<Note> collection,
            IMongoCollection<Workspace> workspaceCollection,
            IPermissionGuard permissionGuard,
            IUserContextProvider userContext)
        {
            _collection = collection;
            _workspaceCollection = workspaceCollection;
            _permissionGuard = permissionGuard;
            _userContext = userContext;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var ownerId = _userContext.UserId;
            if (ownerId == null)
            {
                throw new AppException();
            }

            var workspace = await _workspaceCollection
                .Find(x => x.Id == request.WorkspaceId)
                .FirstOrDefaultAsync(cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>(request.WorkspaceId);
            }

            if (request.Folder != null && !workspace.FolderTree.Exists(request.Folder))
            {
                throw new NotFoundException<Folder>();
            }

            var existingNote = await _collection
                .Find(x => x.Name == request.Name && x.Folder == request.Folder)
                .FirstOrDefaultAsync(cancellationToken);
            if (existingNote != null)
            {
                throw new DuplicationException<Note>();
            }
            
            var note = new Note
            {
                Name = request.Name,
                OwnerId = ownerId,
                WorkspaceId = request.WorkspaceId,
                Folder = request.Folder ?? string.Empty,
                Content = new(),
                SharingInfo = request.SharingInfo ?? new(),
            };

            await _collection.InsertOneAsync(note, cancellationToken: cancellationToken);
            return note.Id;
        }
    }
}
