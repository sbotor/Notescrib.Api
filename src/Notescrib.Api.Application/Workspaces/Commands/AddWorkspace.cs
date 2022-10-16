using Notescrib.Api.Application.Common.Services;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class AddWorkspace
{
    public record Command(string Name, SharingDetails SharingDetails) : ICommand<Result<WorkspaceResponse>>;

    internal class Handler : ICommandHandler<Command, Result<WorkspaceResponse>>
    {
        private readonly IUserContextService _userContextService;
        private readonly IWorkspaceRepository _repository;
        private readonly WorkspaceMapper _mapper;

        public Handler(IUserContextService userContextService, IWorkspaceRepository repository)
        {
            _userContextService = userContextService;
            _repository = repository;
            _mapper = new();
        }

        public async Task<Result<WorkspaceResponse>> Handle(Command request, CancellationToken cancellationToken)
        {
            var ownerId = _userContextService.UserId;
            if (ownerId == null)
            {
                return Result<WorkspaceResponse>.Failure("No user context found.");
            }

            var workspace = _mapper.MapToEntity(request, ownerId);
            var added = await _repository.AddWorkspaceAsync(workspace);

            return Result<WorkspaceResponse>.Created(_mapper.MapToResponse(added));
        }
    }
}
