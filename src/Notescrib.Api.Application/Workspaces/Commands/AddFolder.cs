using Notescrib.Api.Application.Common.Services;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Contracts;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class AddFolder
{
    public record Command(string WorkspaceId, string ParentPath, string Name) : ICommand<Result<FolderResponse>>;

    internal class Handler : ICommandHandler<Command, Result<FolderResponse>>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IPermissionService _permissionService;
        private readonly FolderMapper _mapper;

        public Handler(IWorkspaceRepository repository, IPermissionService permissionService)
        {
            _mapper = new();
            _repository = repository;
            _permissionService = permissionService;
        }

        public async Task<Result<FolderResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = new FolderPath
            {
                ParentPath = request.ParentPath,
                Name = request.Name,
                WorkspaceId = request.WorkspaceId
            };

            var workspace = await _repository.GetWorkspaceByIdAsync(folder.WorkspaceId);
            if (workspace == null)
            {
                return Result<FolderResponse>.NotFound($"Workspace with ID '{folder.WorkspaceId}' not found.");
            }

            if (!_permissionService.CanEdit(workspace.OwnerId))
            {
                return Result<FolderResponse>.Forbidden();
            }

            if (workspace.Folders.Any(x => x.Name == folder.Name))
            {
                return Result<FolderResponse>.Failure($"A folder with name '{folder.Name}' already exists.");
            }

            if (!workspace.Folders.Any(x => x.AbsolutePath == folder.ParentPath))
            {
                return Result<FolderResponse>.NotFound($"Folder with path '{folder.ParentPath}' not found.");
            }

            workspace.Folders.Add(folder);
            await _repository.UpdateWorkspaceAsync(workspace);

            folder = workspace.Folders.First(x => x.Name == folder.Name);
            return Result<FolderResponse>.Success(_mapper.MapToResponse(folder, Enumerable.Empty<NoteOverviewResponse>()));
        }
    }
}
