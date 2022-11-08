using Notescrib.Notes.Application.Common.Providers;
using Notescrib.Notes.Application.Workspaces.Mappers;
using Notescrib.Notes.Core.Entities;
using Notescrib.Notes.Core.Exceptions;
using Notescrib.Notes.Core.Models;

namespace Notescrib.Notes.Application.Workspaces.Commands;

public static class FolderCommandBase
{
    public abstract class Command
    {
        public string? ParentId { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
    
    internal abstract class Handler
    {
        protected IWorkspaceRepository WorkspaceRepository { get; }
        protected IFolderRepository FolderRepository { get; }
        protected ISharingGuard SharingGuard { get; }
        protected IFolderMapper Mapper { get; }

        public Handler(IWorkspaceRepository workspaceRepository, IFolderRepository folderRepository, ISharingGuard sharingGuard, IFolderMapper mapper)
        {
            WorkspaceRepository = workspaceRepository;
            FolderRepository = folderRepository;
            SharingGuard = sharingGuard;
            Mapper = mapper;
        }

        protected static FolderTree.Node FindAndValidateParent(FolderTree tree, string parentId)
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

            SharingGuard.GuardCanEdit(workspace);

            return workspace;
        }
    }
}
