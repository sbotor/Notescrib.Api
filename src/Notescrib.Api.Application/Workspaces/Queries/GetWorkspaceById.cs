using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Queries;

public static class GetWorkspaceById
{
    public record Query(string Id) : IQuery<WorkspaceDetails>;

    internal class Handler : IQueryHandler<Query, WorkspaceDetails>
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

        public async Task<WorkspaceDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetByIdAsync(request.Id);
            if (workspace == null)
            {
                throw new NotFoundException();
            }

            _sharingService.GuardCanView(workspace);

            var folders = await _folderRepository.GetWorkspaceFoldersAsync(workspace.Id!);
            var folderTree = new FolderTree<FolderOverview, Folder>(folders, _folderMapper.MapToOverview);

            var response = _mapper.MapToDetails(workspace, folderTree.Items);
            response.Folders = folderTree.Items.ToArray();

            return response;
        }
    }
}
