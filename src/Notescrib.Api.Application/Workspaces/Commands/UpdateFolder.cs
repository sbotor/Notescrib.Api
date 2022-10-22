using AutoMapper;
using FluentValidation;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

internal static class UpdateFolder
{
    public record Command(string WorkspaceId, string? ParentPath, string Name, SharingDetails SharingDetails) : ICommand<Result<FolderDetails>>;

    internal class Handler : FolderCommandHandlerBase, ICommandHandler<Command, Result<FolderDetails>>
    {
        private readonly IMapper _mapper;

        public Handler(IWorkspaceRepository repository, IPermissionService permissionService, IMapper mapper) : base(repository, permissionService)
        {
            _mapper = mapper;
        }

        public async Task<Result<FolderDetails>> Handle(Command request, CancellationToken cancellationToken)
        {
            return Result<FolderDetails>.Failure();
        }
    }
}
