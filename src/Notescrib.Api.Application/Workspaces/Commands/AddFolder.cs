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
    public record Command(string WorkspaceId, string? ParentPath, string Name) : ICommand<Result<FolderDetails>>;

    internal class Handler : ICommandHandler<Command, Result<FolderDetails>>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IPermissionService _permissionService;
        private readonly IFolderMapper _mapper;

        public Handler(IWorkspaceRepository repository, IPermissionService permissionService, IFolderMapper mapper)
        {
            _mapper = mapper;
            _repository = repository;
            _permissionService = permissionService;
        }

        public async Task<Result<FolderDetails>> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = new Folder
            {
                ParentPath = request.ParentPath,
                Name = request.Name,
                WorkspaceId = request.WorkspaceId
            };

            var workspace = await _repository.GetWorkspaceByIdAsync(folder.WorkspaceId);
            if (workspace == null)
            {
                return Result<FolderDetails>.NotFound($"Workspace with ID '{folder.WorkspaceId}' not found.");
            }

            if (!_permissionService.CanEdit(workspace))
            {
                return Result<FolderDetails>.Forbidden();
            }

            if (workspace.Folders.Any(x => x.Name == folder.Name))
            {
                return Result<FolderDetails>.Failure($"A folder with name '{folder.Name}' already exists.");
            }

            if (!workspace.Folders.Any(x => x.AbsolutePath == folder.ParentPath))
            {
                return Result<FolderDetails>.NotFound($"Folder with path '{folder.ParentPath}' not found.");
            }

            workspace.Folders.Add(folder);
            await _repository.UpdateWorkspaceAsync(workspace);

            folder = workspace.Folders.First(x => x.Name == folder.Name);
            return Result<FolderDetails>.Success(_mapper.MapToResponse(folder, Enumerable.Empty<NoteOverview>()));
        }
    }
}
