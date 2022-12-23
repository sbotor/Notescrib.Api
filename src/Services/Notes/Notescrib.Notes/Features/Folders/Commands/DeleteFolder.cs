using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Folders.Commands;

public static class DeleteFolder
{
    public record Command(string Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly MongoDbContext _context;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(MongoDbContext context, IPermissionGuard permissionGuard)
        {
            _context = context;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _context.Folders.GetByIdAsync(
                request.Id,
                new() { Children = true},
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

            var allFolders = folder.Children.Append(folder).ToArray();
            
            var folderIds = allFolders.Select(x => x.Id).ToArray();

            await _context.EnsureTransactionAsync(CancellationToken.None);
            
            await _context.Folders.DeleteManyAsync(folderIds, CancellationToken.None);
            await _context.Notes.DeleteFromFoldersAsync(_permissionGuard.UserContext.UserId, folderIds, CancellationToken.None);

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
