using MediatR;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class UpdateFolder
{
    public class Command : FolderCommandBase.Command, ICommand
    {
        public string Id { get; set; } = null!;
        public SharingInfo SharingInfo { get; set; } = null!;
    }

    internal class Handler : FolderCommandBase.Handler, ICommandHandler<Command>
    {
        public Handler(IWorkspaceRepository workspaceRepository, IFolderRepository folderRepository, ISharingService sharingService, IFolderMapper mapper)
            : base(workspaceRepository, folderRepository, sharingService, mapper)
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
