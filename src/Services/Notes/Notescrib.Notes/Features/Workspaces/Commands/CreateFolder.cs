using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Workspaces.Models;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class CreateFolder
{
    public record Command(string WorkspaceId, string Name, string? ParentId) : ICommand<FolderOverview>;

    internal class Handler : ICommandHandler<Command, FolderOverview>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Folder, FolderOverview> _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(IWorkspaceRepository repository, IPermissionGuard permissionGuard,
            IMapper<Folder, FolderOverview> mapper, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<FolderOverview> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetWorkspaceByIdAsync(request.WorkspaceId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>(request.WorkspaceId);
            }

            _permissionGuard.GuardCanEdit(workspace.OwnerId);

            var now = _dateTimeProvider.Now;
            workspace.Updated = now;
            
            var folder = new Folder { Id = Guid.NewGuid().ToString(), Name = request.Name, Created = now };
            var tree = new FolderTree(workspace);
            tree.Add(folder, request.ParentId);

            await _repository.UpdateWorkspaceAsync(workspace, cancellationToken);

            return _mapper.Map(folder);
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.WorkspaceId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Counts.Name.MaxLength);

            RuleFor(x => x.ParentId)
                .NotEmpty()
                .When(x => x.ParentId != null);
        }
    }
}
