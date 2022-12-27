using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Folders.Commands;

public static class UpdateFolder
{
    public record Command(string FolderId, string Name) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly MongoDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(
            MongoDbContext context,
            IDateTimeProvider dateTimeProvider,
            IPermissionGuard permissionGuard)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _context.Folders.GetByIdAsync(request.FolderId, cancellationToken: cancellationToken);
            if (folder == null)
            {
                throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);
            }
            
            _permissionGuard.GuardCanEdit(folder.OwnerId);

            if (folder.ParentId == null)
            {
                throw new AppException("Cannot update root folder.");
            }

            // TODO: optimize this later.
            var parent = await _context.Folders.GetByIdAsync(
                    folder.ParentId,
                    new() { Children = true },
                    cancellationToken);
            if (parent == null)
            {
                throw new NotFoundException(ErrorCodes.Folder.FolderNotFound, "Parent folder does not exist.");
            }

            if (parent.Children.Any(x => x.Name == request.Name))
            {
                throw new DuplicationException(ErrorCodes.Folder.FolderAlreadyExists);
            }

            var now = _dateTimeProvider.Now;
            folder.Updated = now;
            folder.Name = request.Name;

            await _context.Folders.UpdateAsync(folder, cancellationToken);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FolderId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Consts.Name.MaxLength);
        }
    }
}
