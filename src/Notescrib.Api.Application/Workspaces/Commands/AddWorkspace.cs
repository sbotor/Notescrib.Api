using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class AddWorkspace
{
    public record Command(string Name, SharingDetails SharingDetails) : ICommand<Result<WorkspaceDetails>>;

    internal class Handler : ICommandHandler<Command, Result<WorkspaceDetails>>
    {
        private readonly IUserContextService _userContextService;
        private readonly IWorkspaceRepository _repository;
        private readonly IWorkspaceMapper _mapper;

        public Handler(IUserContextService userContextService, IWorkspaceRepository repository, IWorkspaceMapper mapper)
        {
            _userContextService = userContextService;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<WorkspaceDetails>> Handle(Command request, CancellationToken cancellationToken)
        {
            var ownerId = _userContextService.UserId;
            if (ownerId == null)
            {
                return Result<WorkspaceDetails>.Failure("No user context found.");
            }

            var workspace = _mapper.MapToEntity(request, ownerId);
            var added = await _repository.AddWorkspaceAsync(workspace);

            return Result<WorkspaceDetails>.Created(_mapper.MapToResponse(added));
        }
    }
}
