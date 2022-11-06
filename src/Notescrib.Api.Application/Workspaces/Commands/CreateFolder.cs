using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class CreateFolder
{
    public class Command : FolderCommandBase.Command, ICommand<Result<string>>
    {
        public string WorkspaceId { get; set; }
    }

    internal class Handler : FolderCommandBase.Handler, ICommandHandler<Command, Result<string>>
    {
        public Handler(
            IWorkspaceRepository workspaceRepository,
            IFolderRepository folderRepository,
            ISharingService sharingService,
            IFolderMapper mapper)
            : base(workspaceRepository, folderRepository, sharingService, mapper)
        {
        }

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspaceResult = await FindAndValidateWorkspace(request.WorkspaceId);
            if (!workspaceResult.IsSuccessful)
            {
                return workspaceResult.CastError<string>();
            }

            var workspace = workspaceResult.Response!;

            var folders = await FolderRepository.GetWorkspaceFoldersAsync(request.WorkspaceId);
            var tree = new FolderTree(folders);

            if (tree.Any(x => x.Item.Name == request.Name))
            {
                return Result<string>.Failure("Folder with this name already exists.");
            }

            Folder? parent = null;
            if (request.ParentId != null)
            {
                var parentResult = FindAndValidateParent(tree, request.ParentId);
                if (!parentResult.IsSuccessful)
                {
                    return parentResult.CastError<string>();
                }

                parent = parentResult.Response!.Item;
            }

            var folder = Mapper.Map<Folder>(request);
            
            folder.SharingInfo = request.SharingInfo
                                 ?? parent?.SharingInfo
                                 ?? workspace.SharingInfo;

            folder.OwnerId = workspace.OwnerId;

            await FolderRepository.AddAsync(folder);
            return Result<string>.Success(folder.Id);
        }
    }
}
