using MediatR;
using Notescrib.Notes.Application.Common.Providers;
using Notescrib.Notes.Application.Workspaces.Mappers;
using Notescrib.Notes.Core.Entities;
using Notescrib.Notes.Core.Exceptions;
using Notescrib.Notes.Core.Models;

namespace Notescrib.Notes.Application.Workspaces.Commands;

public static class CreateFolder
{
    public class Command : FolderCommandBase.Command, IRequest<string>
    {
        public string WorkspaceId { get; set; }
        
        public SharingInfo? SharingInfo { get; set; }
    }

    internal class Handler : FolderCommandBase.Handler, IRequestHandler<Command, string>
    {
        public Handler(
            IWorkspaceRepository workspaceRepository,
            IFolderRepository folderRepository,
            ISharingGuard sharingGuard,
            IFolderMapper mapper)
            : base(workspaceRepository, folderRepository, sharingGuard, mapper)
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
