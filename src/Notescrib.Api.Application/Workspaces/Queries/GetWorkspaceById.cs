using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Queries;

public static class GetWorkspaceById
{
    public record Query(string Id) : IQuery<Result<WorkspaceDetails>>;

    internal class Handler : IQueryHandler<Query, Result<WorkspaceDetails>>
    {
        private readonly ISharingService _sharingService;
        private readonly IWorkspaceRepository _repository;
        private readonly IFolderRepository _folderRepository;
        private readonly IWorkspaceMapper _mapper;
        private readonly IFolderMapper _folderMapper;

        public Handler(
            ISharingService sharingService,
            IWorkspaceRepository repository,
            IFolderRepository folderRepository,
            IWorkspaceMapper mapper,
            IFolderMapper folderMapper)
        {
            _sharingService = sharingService;
            _repository = repository;
            _folderRepository = folderRepository;
            _mapper = mapper;
            _folderMapper = folderMapper;
        }

        public async Task<Result<WorkspaceDetails>> Handle(Query request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetByIdAsync(request.Id);
            if (workspace == null)
            {
                return Result<WorkspaceDetails>.NotFound();
            }

            if (!_sharingService.CanView(workspace))
            {
                return Result<WorkspaceDetails>.Forbidden();
            }

            var folders = await _folderRepository.GetWorkspaceFoldersAsync(workspace.Id!);
            folders = folders.Where(x => _sharingService.CanView(x)).ToList();
            
            var folderTree = _folderMapper.MapToTree(folders);

            var response = _mapper.Map<WorkspaceDetails>(workspace);
            response.Folders = folderTree;

            return Result<WorkspaceDetails>.Success(response);
        }
    }
}
