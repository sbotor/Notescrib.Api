using MediatR;
using MongoDB.Driver;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Workspaces.Models;
using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Exceptions;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Queries;

public static class GetWorkspaces
{
    public record Query(Paging Paging, Sorting Sorting) : IPagingSortingRequest<Workspace, PagedList<WorkspaceOverview>>;

    internal class Handler : IRequestHandler<Query, PagedList<WorkspaceOverview>>
    {
        private readonly IMongoCollection<Workspace> _collection;
        private readonly IUserContextProvider _userContext;
        private readonly IMapper<Workspace, WorkspaceOverview> _mapper;

        public Handler(IMongoCollection<Workspace> collection, IUserContextProvider userContext, IMapper<Workspace, WorkspaceOverview> mapper)
        {
            _collection = collection;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<PagedList<WorkspaceOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var ownerId = _userContext.UserId;
            if (ownerId == null)
            {
                throw new AppException("No user context found.");
            }

            var result = await _collection.FindPagedAsync(
                x => x.OwnerId == ownerId,
                request.Paging,
                request.Sorting);

            return result.Map(_mapper.Map);
        }
    }
}
