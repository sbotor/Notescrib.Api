using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class AddFolder
{
    public record Command(string WorkspaceId, string? ParentId, string Name, SharingDetails? SharingDetails) : ICommand<Result<string>>;

    internal class Handler : FolderCommandHandlerBase, ICommandHandler<Command, Result<string>>
    {
        public Handler(IWorkspaceRepository workspaceRepository, IFolderRepository folderRepository, IPermissionService permissionService, IFolderMapper mapper)
            : base(workspaceRepository, folderRepository, permissionService, mapper)
        {
        }

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = Mapper.Map<Folder>(request);

            var workspaceResult = await GetWorkspace(folder);
            if (!workspaceResult.IsSuccessful || workspaceResult.Response == null)
            {
                return workspaceResult.Map<string>();
            }

            var folders = await FolderRepository.GetWorkspaceFoldersAsync(folder.WorkspaceId);
            
            if (folders.Any(x => x.Name == folder.Name))
            {
                return Result<string>.Failure("Folder with this name already exists.");
            }

            var parent = folders.FirstOrDefault(x => x.Id == folder.ParentId);
            if (folder.ParentId != null && parent == null)
            {
                return Result<string>.NotFound("Parent folder does not exist.");
            }

            var workspace = workspaceResult.Response;

            folder.SharingDetails = request.SharingDetails
                ?? parent?.SharingDetails
                ?? workspace.SharingDetails;
            folder.OwnerId = workspace.OwnerId;

            await FolderRepository.AddFolderAsync(folder);
            return Result<string>.Success(folder.Id);
        }
    }
}
