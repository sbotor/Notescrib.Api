using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Notes;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Queries;

public static class GetWorkspaceById
{
    public record Query(string Id, IPaging NotePaging) : IQuery<Result<WorkspaceDetails>>;

    internal class Handler : IQueryHandler<Query, Result<WorkspaceDetails>>
    {
        private readonly IPermissionService _permissionService;
        private readonly IWorkspaceRepository _repository;
        private readonly IFolderRepository _folderRepository;
        private readonly INoteRepository _noteRepository;
        private readonly IWorkspaceMapper _mapper;
        private readonly IFolderMapper _folderMapper;

        public Handler(
            IPermissionService permissionService,
            IWorkspaceRepository repository,
            IFolderRepository folderRepository,
            INoteRepository noteRepository,
            IWorkspaceMapper mapper,
            IFolderMapper folderMapper)
        {
            _permissionService = permissionService;
            _repository = repository;
            _folderRepository = folderRepository;
            _noteRepository = noteRepository;
            _mapper = mapper;
            _folderMapper = folderMapper;
        }

        public async Task<Result<WorkspaceDetails>> Handle(Query request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetWorkspaceByIdAsync(request.Id);
            if (workspace == null)
            {
                return Result<WorkspaceDetails>.NotFound();
            }

            if (!_permissionService.CanView(workspace))
            {
                return Result<WorkspaceDetails>.Forbidden();
            }

            var folders = await _folderRepository.GetWorkspaceFoldersAsync(workspace.Id!);
            folders = folders.Where(x => _permissionService.CanView(x)).ToList();

            var rootFolder = folders.First(x => x.IsRoot);
            var notes = await _noteRepository.GetNotesFromTreeAsync(rootFolder.Id!, request.NotePaging);

            var response = _folderMapper.Map<WorkspaceDetails>(workspace);

            return Result<WorkspaceOverview>.Success(_mapper.Map<WorkspaceOverview>(workspace));
        }
    }
}
