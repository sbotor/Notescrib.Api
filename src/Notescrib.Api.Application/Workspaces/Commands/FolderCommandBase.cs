using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;
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

        protected FolderTree.Node FindAndValidateParent(FolderTree tree, string parentId)
        {
            var parentNode = tree.FirstOrDefault(x => x.Item.Id == parentId);
        
            if (parentNode == null)
            {
                throw new NotFoundException("Parent folder does not exist.");
            }

            return !parentNode.CanNestChildren
                ? throw new AppException("Max nesting level achieved.")
                : parentNode;
        }

        protected async Task<Workspace> FindAndValidateWorkspace(string workspaceId)
        {
            var workspace = await WorkspaceRepository.GetByIdAsync(workspaceId);
            if (workspace == null)
            {
                throw new NotFoundException("Workspace not found.");
            }

            SharingService.GuardCanEdit(workspace);

            return workspace;
        }
    }
}
