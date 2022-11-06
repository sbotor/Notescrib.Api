using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class CreateFolder
{
    public class Command : FolderCommandBase.Command, ICommand<string>
    {
        public string WorkspaceId { get; set; }
        
        public SharingInfo? SharingInfo { get; set; }
    }

    internal class Handler : FolderCommandBase.Handler, ICommandHandler<Command, string>
    {
        public Handler(
            IWorkspaceRepository workspaceRepository,
            IFolderRepository folderRepository,
            ISharingService sharingService,
            IFolderMapper mapper)
            : base(workspaceRepository, folderRepository, sharingService, mapper)
        {
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await FindAndValidateWorkspace(request.WorkspaceId);

            var folders = await FolderRepository.GetWorkspaceFoldersAsync(request.WorkspaceId);
            var tree = new FolderTree(folders);

            if (tree.Any(x => x.Item.Name == request.Name))
            {
                throw new AppException("Folder with this name already exists.");
            }

            Folder? parent = null;
            if (request.ParentId != null)
            {
                parent = FindAndValidateParent(tree, request.ParentId).Item;
            }

            var folder = Mapper.MapToEntity(
                request,
                workspace.OwnerId,
                request.SharingInfo ?? parent?.SharingInfo ?? workspace.SharingInfo);

            return (await FolderRepository.AddAsync(folder)).Id;
        }
    }
}
