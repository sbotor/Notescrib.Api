using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Folders.Queries;

public static class GetFolderTree
{
    public record Query : IQuery<IReadOnlyCollection<FolderOverview>>;

    internal class Handler : IQueryHandler<Query, IReadOnlyCollection<FolderOverview>>
    {
        private readonly IPermissionGuard _permissionGuard;
        private readonly IWorkspaceRepository _repository;
        private readonly IMapper<Folder, FolderOverview> _mapper;

        public Handler(IPermissionGuard permissionGuard, IWorkspaceRepository repository, IMapper<Folder, FolderOverview> mapper)
        {
            _permissionGuard = permissionGuard;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<FolderOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = _permissionGuard.UserContext.UserId;
            var workspace = await _repository.GetByOwnerIdAsync(userId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }
            
            _permissionGuard.GuardCanView(workspace.OwnerId);

            return workspace.FolderTree.Children.MapTree(_mapper.Map).ToArray();
        }
    }
}
