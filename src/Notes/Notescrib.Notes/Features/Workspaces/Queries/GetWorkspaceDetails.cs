using MediatR;
using MongoDB.Driver;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Workspaces.Models;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Queries;

public static class GetWorkspaceDetails
{
    public record Query(string Id) : IRequest<WorkspaceDetails>;

    internal class Handler : IRequestHandler<Query, WorkspaceDetails>
    {
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMongoCollection<Workspace> _collection;
        private readonly IMapper<Workspace, WorkspaceDetails> _mapper;

        public Handler(IPermissionGuard permissionGuard, IMongoCollection<Workspace> collection, IMapper<Workspace, WorkspaceDetails> mapper)
        {
            _permissionGuard = permissionGuard;
            _collection = collection;
            _mapper = mapper;
        }
        
        public async Task<WorkspaceDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var workspace = await _collection
                .Find(x => x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }
            
            _permissionGuard.GuardCanView(workspace.OwnerId);

            return _mapper.Map(workspace);
        }
    }
}
