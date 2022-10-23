using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class AddFolder
{
    public record Command(string WorkspaceId, string? ParentPath, string Name, SharingDetails? SharingDetails) : ICommand<Result<string>>;

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

            var workspace = workspaceResult.Response;
            workspace.SharingDetails = request.SharingDetails ?? workspace.SharingDetails;

            var folders = await FolderRepository.GetWorkspaceFoldersAsync(folder.WorkspaceId);
            
            if (folders.Any(x => x.Name == folder.Name))
            {
                return Result<string>.Failure("Folder with this name already exists.");
            }

            if (folder.ParentPath != null && !folders.Any(x => x.AbsolutePath == folder.ParentPath))
            {
                return Result<string>.NotFound("Parent folder does not exist.");
            }

            await FolderRepository.AddFolderAsync(folder);
            return Result<string>.Success(folder.Id);
        }
    }
}
