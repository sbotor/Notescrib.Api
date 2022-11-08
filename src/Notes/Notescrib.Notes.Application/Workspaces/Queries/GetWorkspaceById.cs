using MediatR;
using Notescrib.Notes.Application.Common.Providers;
using Notescrib.Notes.Application.Workspaces.Mappers;
using Notescrib.Notes.Application.Workspaces.Models;
using Notescrib.Notes.Core.Entities;
using Notescrib.Notes.Core.Exceptions;
using Notescrib.Notes.Core.Models;

namespace Notescrib.Notes.Application.Workspaces.Queries;

public static class GetWorkspaceById
{
    public record Query(string Id) : IRequest<WorkspaceDetails>;

    internal class Handler : IRequestHandler<Query, WorkspaceDetails>
    {
        private readonly ISharingGuard _sharingGuard;
        private readonly IWorkspaceRepository _repository;
        private readonly IFolderRepository _folderRepository;
        private readonly IWorkspaceMapper _mapper;
        private readonly IFolderMapper _folderMapper;

        public Handler(
            ISharingGuard sharingGuard,
            IWorkspaceRepository repository,
            IFolderRepository folderRepository,
            IWorkspaceMapper mapper,
            IFolderMapper folderMapper)
        {
            _sharingGuard = sharingGuard;
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

            _sharingGuard.GuardCanView(workspace);

            var folders = await _folderRepository.GetWorkspaceFoldersAsync(workspace.Id!);
            var folderTree = new FolderTree<FolderOverview, Folder>(folders, _folderMapper.MapToOverview);

            var response = _mapper.MapToDetails(workspace, folderTree.Items);
            response.Folders = folderTree.Items.ToArray();

            return response;
        }
    }
}
