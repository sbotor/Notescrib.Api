using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class AddWorkspace
{
    public record Command(string Name, SharingDetails SharingDetails) : ICommand<Result<string>>;

    internal class Handler : ICommandHandler<Command, Result<string>>
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

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var ownerId = _userContextService.UserId;
            if (ownerId == null)
            {
                return Result<string>.Failure("No user context found.");
            }

            var workspace = _mapper.MapToEntity(request, ownerId);
            await _repository.AddWorkspaceAsync(workspace);

            return Result<string>.Created(workspace.Id);
        }
    }
}
