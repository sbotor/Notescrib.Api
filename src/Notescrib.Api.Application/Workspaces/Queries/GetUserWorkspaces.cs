using Notescrib.Api.Application.Common.Services;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Mappers;
using Notescrib.Api.Application.Workspaces.Contracts;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Queries;

public static class GetUserWorkspaces
{
    public record Query : IQuery<Result<IReadOnlyCollection<WorkspaceResponse>>>;

    internal class Handler : IQueryHandler<Query, Result<IReadOnlyCollection<WorkspaceResponse>>>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IUserContextService _userContextService;
        private readonly IWorkspaceMapper _mapper;

        public Handler(IWorkspaceRepository repository, IUserContextService userContextService, IWorkspaceMapper mapper)
        {
            _repository = repository;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public async Task<Result<IReadOnlyCollection<WorkspaceResponse>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var ownerId = _userContextService.UserId;
            if (ownerId == null)
            {
                return Result<IReadOnlyCollection<WorkspaceResponse>>.Failure();
            }

            var result = await _repository.GetUserWorkspaces(ownerId);
            var response = result.Select(x => _mapper.MapToResponse(x)).ToList();

            return Result<IReadOnlyCollection<WorkspaceResponse>>.Success(response);
        }
    }
}
