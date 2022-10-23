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
        private readonly IFolderRepository _folderRepository;
        private readonly IWorkspaceMapper _mapper;

        public Handler(IUserContextService userContextService, IWorkspaceRepository repository, IFolderRepository folderRepository, IWorkspaceMapper mapper)
        {
            _userContextService = userContextService;
            _repository = repository;
            _folderRepository = folderRepository;
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
            var workspaceId = await _repository.AddWorkspaceAsync(workspace);

            var folder = new Folder
            {
                Id = workspaceId,
                Name = string.Empty,
                OwnerId = ownerId,
                WorkspaceId = workspaceId,
                SharingDetails = workspace.SharingDetails
            };
            await _folderRepository.AddFolderAsync(folder);

            return Result<string>.Created(workspace.Id);
        }
    }
}
