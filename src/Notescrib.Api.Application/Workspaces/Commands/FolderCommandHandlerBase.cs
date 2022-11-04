using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

internal abstract class FolderCommandHandlerBase
{
    protected IWorkspaceRepository WorkspaceRepository { get; }
    protected IFolderRepository FolderRepository { get; }
    protected ISharingService SharingService { get; }
    protected IFolderMapper Mapper { get; }

    public FolderCommandHandlerBase(IWorkspaceRepository workspaceRepository, IFolderRepository folderRepository, ISharingService sharingService, IFolderMapper mapper)
    {
        WorkspaceRepository = workspaceRepository;
        FolderRepository = folderRepository;
        SharingService = sharingService;
        Mapper = mapper;
    }

    protected async Task<Result<Workspace>> GetWorkspace(Folder folder)
    {
        var workspace = await WorkspaceRepository.GetByIdAsync(folder.WorkspaceId);
        if (workspace == null)
        {
            return Result<Workspace>.NotFound($"Workspace not found.");
        }

        return !SharingService.CanEdit(workspace)
            ? Result<Workspace>.Forbidden()
            : Result<Workspace>.Success(workspace);
    }
}
