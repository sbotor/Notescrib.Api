using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data.MongoDb;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Folders.Commands;

public static class DeleteFolder
{
    public record Command(string Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IMongoDbContext _context;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(IMongoDbContext context, IPermissionGuard permissionGuard)
        {
            _context = context;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _context.Folders.GetByIdAsync(
                request.Id,
                new() { Children = true },
                cancellationToken);
            if (folder == null)
            {
                throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);
            }

            _permissionGuard.GuardCanEdit(folder.OwnerId);

            if (folder.ParentId == null)
            {
                throw new AppException("Cannot delete root folder.");
            }

            var allFolderIds = folder.Children.Append(folder).Select(x => x.Id).ToArray();

            await _context.EnsureTransactionAsync(CancellationToken.None);

            await _context.Folders.DeleteManyAsync(allFolderIds, CancellationToken.None);

            var noteIds = await _context.Notes.GetIdsFromFoldersAsync(allFolderIds, CancellationToken.None);
            await _context.Notes.DeleteFromRelatedAsync(noteIds, CancellationToken.None);
            await _context.Notes.DeleteManyAsync(noteIds, CancellationToken.None);

            await _context.CommitTransactionAsync();

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
