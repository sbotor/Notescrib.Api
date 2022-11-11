using MediatR;
using MongoDB.Driver;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class CreateFolder
{
    public record Command(string WorkspaceId, string Name, string? Parent) : IRequest;

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
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>(request.WorkspaceId);
            }
            
            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            var folder = new Folder { Name = request.Name };
            workspace.FolderTree.Add(folder, request.Parent);

            await _collection.ReplaceOneAsync(x => x.Id == workspace.Id, workspace,
                cancellationToken: cancellationToken);

            return Unit.Value;
        }
    }
}
