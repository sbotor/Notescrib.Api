using MediatR;
using MongoDB.Driver;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class CreateFolder
{
    public record Command(string WorkspaceId, string Name, string? Parent, SharingInfo? SharingInfo) : IRequest;

    internal class Handler : IRequestHandler<Command>
    {
        private readonly IMongoCollection<Workspace> _collection;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(IMongoCollection<Workspace> collection, IPermissionGuard permissionGuard)
        {
            _collection = collection;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _collection
                .Find(x => x.Id == request.WorkspaceId)
                .FirstOrDefaultAsync(cancellationToken);
            
            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            var folder = new Folder { Name = request.Name };
            var node = workspace.FolderTree.Add(folder, request.Parent);

            folder.SharingInfo = request.SharingInfo
                                 ?? node.Parent?.Item.SharingInfo
                                 ?? workspace.SharingInfo;
            
            await _collection.FindOneAndReplaceAsync(x => x.Id == workspace.Id, workspace,
                cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
