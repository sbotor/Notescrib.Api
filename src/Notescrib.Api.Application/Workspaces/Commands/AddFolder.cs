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
    public record Command(string WorkspaceId, string? ParentPath, string Name, SharingDetails? SharingDetails) : ICommand<Result<FolderDetails>>;

    internal class Handler : FolderCommandHandlerBase, ICommandHandler<Command, Result<FolderDetails>>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IFolderMapper _mapper;

        public Handler(IWorkspaceRepository repository, IPermissionService permissionService, IFolderMapper mapper)
            : base(repository, permissionService)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<Result<FolderDetails>> Handle(Command request, CancellationToken cancellationToken)
        {
            //var folder = new Folder
            //{
            //    ParentPath = request.ParentPath,
            //    Name = request.Name,
            //    WorkspaceId = request.WorkspaceId
            //};

            var folder = _mapper.Map<Folder>(request);

            var workspaceResult = await ValidateAndGetWorkspace(folder);
            if (!workspaceResult.IsSuccessful || workspaceResult.Response == null)
            {
                return workspaceResult.Map<FolderDetails>();
            }

            var workspace = workspaceResult.Response;

            workspace.SharingDetails = request.SharingDetails ?? workspace.SharingDetails;

            workspace.Folders.Add(folder);
            await _repository.UpdateWorkspaceAsync(workspace);

            folder = workspace.Folders.First(x => x.Name == folder.Name);
            return Result<FolderDetails>.Success(_mapper.MapToResponse(folder, Enumerable.Empty<NoteOverview>()));
        }
    }
}
