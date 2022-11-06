using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class UpdateFolder
{
    public class Command : FolderCommandBase.Command, IQuery<Result>
    {
        public string Id { get; set; } = null!;
    }

    internal class Handler : FolderCommandBase.Handler, IQueryHandler<Command, Result>
    {
        public Handler(IWorkspaceRepository workspaceRepository, IFolderRepository folderRepository, ISharingService sharingService, IFolderMapper mapper)
            : base(workspaceRepository, folderRepository, sharingService, mapper)
        {
        }
        
        public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await FolderRepository.GetByIdAsync(request.Id);
            if (folder == null)
            {
                return Result.NotFound();
            }

            var workspaceResult = await FindAndValidateWorkspace(folder.WorkspaceId);
            if (!workspaceResult.IsSuccessful)
            {
                return workspaceResult.CastError();
            }

            var folders = await FolderRepository.GetWorkspaceFoldersAsync(folder.WorkspaceId);
            var tree = new FolderTree(folders);
            
            if (request.ParentId != null)
            {
                var parentResult = FindAndValidateParent(tree, request.ParentId);
                if (!parentResult.IsSuccessful)
                {
                    return parentResult.CastError();
                }
                
                var parentNode = parentResult.Response!;
                if (parentNode.FindAncestor(x => x.Item.Id == folder.Id) != null)
                {
                    return Result<string>.Failure("The folder cannot be its own ancestor.");
                }
            }

            folder = Mapper.Update(folder, request);

            await FolderRepository.UpdateAsync(folder);
            return Result.Success();
        }
    }
}
