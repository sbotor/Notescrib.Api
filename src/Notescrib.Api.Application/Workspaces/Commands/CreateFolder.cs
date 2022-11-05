using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class CreateFolder
{
    public record Command(string WorkspaceId, string? ParentId, string Name, SharingInfo? SharingInfo) : ICommand<Result<string>>;

    internal class Handler : FolderCommandHandlerBase, ICommandHandler<Command, Result<string>>
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
            var folder = Mapper.Map<Folder>(request);

            var workspaceResult = await GetWorkspace(folder.WorkspaceId);
            if (!workspaceResult.IsSuccessful || workspaceResult.Response == null)
            {
                return workspaceResult.Map<string>();
            }

            var folders = await FolderRepository.GetWorkspaceFoldersAsync(folder.WorkspaceId);
            var folderTree = new FolderTree(folders);
            
            if (folderTree.Any(x => x.Item.Name == folder.Name))
            {
                return Result<string>.Failure("Folder with this name already exists.");
            }

            Folder? parent = null;
            if (folder.ParentId != null)
            {
                var parentNode = folderTree.FirstOrDefault(x => x.Item.Id == folder.ParentId);
                if (parentNode == null)
                {
                    return Result<string>.NotFound("Parent folder does not exist.");
                }

                if (!parentNode.CanNestChildren)
                {
                    return Result<string>.Failure("Max nesting level achieved.");
                }

                if (parentNode.FindAncestor(x => x.Item.Id == folder.Id) != null)
                {
                    return Result<string>.Failure("The folder cannot be its own ancestor.");
                }

                parent = parentNode.Item;
            }

            var workspace = workspaceResult.Response;

            folder.SharingInfo = request.SharingInfo
                ?? parent?.SharingInfo
                ?? workspace.SharingInfo;

            folder.NestingLevel = (parent?.NestingLevel + 1) ?? 0;

            folder.OwnerId = workspace.OwnerId;

            await FolderRepository.AddAsync(folder);
            return Result<string>.Success(folder.Id);
        }
    }
}
