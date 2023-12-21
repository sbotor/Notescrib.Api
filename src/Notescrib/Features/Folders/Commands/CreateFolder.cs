using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Notes.Data.MongoDb;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Folders.Commands;

public static class CreateFolder
{
    public record Command(string Name, string? ParentId) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IMongoDbContext _context;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            IMongoDbContext context,
            IPermissionGuard permissionGuard,
            IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _permissionGuard.UserContext.UserId;
            var now = _dateTimeProvider.Now;

            var includeOptions = new FolderIncludeOptions { Children = true };

            var parent = request.ParentId == null
                ? await _context.Folders.GetRootAsync(includeOptions, cancellationToken)
                : await _context.Folders.GetByIdAsync(request.ParentId, includeOptions, cancellationToken);
            if (parent == null)
            {
                throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);
            }

            _permissionGuard.GuardCanEdit(parent.OwnerId);
            
            if (parent.Children.Count >= Consts.Folder.MaxChildrenCount)
            {
                throw new AppException(ErrorCodes.Folder.MaximumFolderCountReached);
            }

            if (parent.AncestorIds.Count >= Consts.Folder.MaxNestingLevel)
            {
                throw new AppException(ErrorCodes.Folder.CannotNestMoreChildren);
            }

            if (parent.Children.Any(x => x.Name == request.Name))
            {
                throw new DuplicationException(ErrorCodes.Folder.FolderAlreadyExists);
            }
            
            var folder = new Folder
            {
                Name = request.Name,
                Created = now,
                OwnerId = userId,
                AncestorIds = parent.AncestorIds.Append(parent.Id).ToArray(),
                ParentId = parent.Id,
                WorkspaceId = parent.WorkspaceId
            };

            await _context.Folders.CreateAsync(folder, cancellationToken);

            return Unit.Value;
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
