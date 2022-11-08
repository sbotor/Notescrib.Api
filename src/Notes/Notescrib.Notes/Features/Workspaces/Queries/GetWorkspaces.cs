using MediatR;
using MongoDB.Driver;
using Notescrib.Notes.Application.Contracts;
using Notescrib.Notes.Application.Extensions;
using Notescrib.Notes.Application.Features.Workspaces.Models;
using Notescrib.Notes.Application.Models;
using Notescrib.Notes.Application.Models.Exceptions;
using Notescrib.Notes.Application.Services;

namespace Notescrib.Notes.Application.Features.Workspaces.Queries;

public static class GetWorkspaces
{
    public record Query(Paging Paging, Sorting Sorting) : IRequest<PagedList<WorkspaceOverview>>;

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
