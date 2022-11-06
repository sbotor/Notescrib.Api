using System.Security.AccessControl;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class FolderCommandBase
{
    public abstract class Command
    {
        public string? ParentId { get; set; } = null!;
        public string Name { get; set; } = null!;
        public SharingInfo? SharingInfo { get; set; }
    }
    
    internal abstract class Handler
    {
        protected IWorkspaceRepository WorkspaceRepository { get; }
        protected IFolderRepository FolderRepository { get; }
        protected ISharingService SharingService { get; }
        protected IFolderMapper Mapper { get; }

        public Handler(IWorkspaceRepository workspaceRepository, IFolderRepository folderRepository, ISharingService sharingService, IFolderMapper mapper)
        {
            WorkspaceRepository = workspaceRepository;
            FolderRepository = folderRepository;
            SharingService = sharingService;
            Mapper = mapper;
        }

        protected Result<FolderTree.Node?> FindAndValidateParent(FolderTree tree, string parentId)
        {
            var parentNode = tree.FirstOrDefault(x => x.Item.Id == parentId);
        
            if (parentNode == null)
            {
                return Result<FolderTree.Node?>.NotFound("Parent folder does not exist.");
            }

            return !parentNode.CanNestChildren
                ? Result<FolderTree.Node?>.Failure("Max nesting level achieved.")
                : Result<FolderTree.Node?>.Success(parentNode);
        }

        protected async Task<Result<Workspace>> FindAndValidateWorkspace(string workspaceId)
        {
            var workspace = await WorkspaceRepository.GetByIdAsync(workspaceId);
            if (workspace == null)
            {
                return Result<Workspace>.NotFound($"Workspace not found.");
            }

            if (!SharingService.CanEdit(workspace))
            {
                return Result<Workspace>.Forbidden();
            }

            return Result<Workspace>.Success(workspace);
        }
    }
}
