using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Folders.Commands;

public static class CreateFolder
{
    public record Command(string Name, string ParentId) : ICommand<FolderInfoBase>;

    internal class Handler : ICommandHandler<Command, FolderInfoBase>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Folder, FolderInfoBase> _mapper;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(IWorkspaceRepository repository, IPermissionGuard permissionGuard,
            IMapper<Folder, FolderOverview> mapper, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<FolderInfoBase> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _permissionGuard.UserContext.UserId;
            var workspace = await _repository.GetByOwnerIdAsync(userId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }

            _permissionGuard.GuardCanEdit(workspace.OwnerId);
            
            if (workspace.FolderCount >= Consts.Workspace.MaxFolderCount)
            {
                throw new AppException("Maximum folder count reached.");
            }
            
            var now = _dateTimeProvider.Now;
            
            var folder = new Folder { Id = Guid.NewGuid().ToString(), Name = request.Name, Created = now };
            var tree = new Tree<Folder>(workspace.FolderTree);

            var found = tree.VisitBreadthFirst(x =>
            {
                if (x.Item.Id != request.ParentId)
                {
                    return false;
                }

                if (x.Level >= Consts.Folder.MaxNestingLevel)
                {
                    throw new AppException("The parent cannot nest children.");
                }
                    
                x.Item.ChildrenIds.Add(folder);
                workspace.FolderCount++;
                
                return true;
            });
            
            if (!found)
            {
                throw new NotFoundException<Folder>();
            }

            await _repository.UpdateAsync(workspace, cancellationToken);

            return _mapper.Map(folder);
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Consts.Name.MaxLength);

            RuleFor(x => x.ParentId)
                .NotEmpty();
        }
    }
}
