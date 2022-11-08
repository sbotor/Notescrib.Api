using MediatR;
using Notescrib.Notes.Application.Common.Providers;
using Notescrib.Notes.Application.Workspaces.Mappers;
using Notescrib.Notes.Core.Entities;
using Notescrib.Notes.Core.Exceptions;
using Notescrib.Notes.Core.Models;

namespace Notescrib.Notes.Application.Workspaces.Commands;

public static class UpdateFolder
{
    public class Command : FolderCommandBase.Command, IRequest
    {
        public string Id { get; set; } = null!;
        public SharingInfo SharingInfo { get; set; } = null!;
    }

    internal class Handler : FolderCommandBase.Handler, IRequestHandler<Command>
    {
        public Handler(IWorkspaceRepository workspaceRepository, IFolderRepository folderRepository, ISharingGuard sharingGuard, IFolderMapper mapper)
            : base(workspaceRepository, folderRepository, sharingGuard, mapper)
        {
        }
        
        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await FolderRepository.GetByIdAsync(request.Id);
            if (folder == null)
            {
                throw new NotFoundException();
            }

            await FindAndValidateWorkspace(folder.WorkspaceId);

            var folders = await FolderRepository.GetWorkspaceFoldersAsync(folder.WorkspaceId);
            var tree = new FolderTree(folders);
            
            if (request.ParentId != null)
            {
                var parentNode = FindAndValidateParent(tree, request.ParentId);
                
                if (parentNode.FindAncestor(x => x.Item.Id == folder.Id) != null)
                {
                    throw new AppException("The folder cannot be its own ancestor.");
                }
            }

            folder = Mapper.UpdateEntity(request, folder);

            await FolderRepository.UpdateAsync(folder);
            return Unit.Value;
        }
    }
}
