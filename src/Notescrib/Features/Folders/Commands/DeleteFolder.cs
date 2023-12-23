using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Folders.Commands;

public static class DeleteFolder
{
    public record Command(Guid Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(NotescribDbContext dbContext, IPermissionGuard permissionGuard)
        {
            _dbContext = dbContext;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _dbContext.Folders.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);

            await _permissionGuard.GuardCanEdit(folder.OwnerId);

            if (folder.ParentId == null)
            {
                throw new AppException("Cannot delete root folder.");
            }

            var queue = new Queue<Guid>();
            queue.Enqueue(folder.Id);

            var folderIds = new List<Guid>();

            while (queue.Count > 0)
            {
                var id = queue.Dequeue();
                folderIds.Add(id);

                var childrenEnumerable = _dbContext.Folders.AsNoTracking()
                    .Where(x => x.ParentId == id)
                    .Select(x => x.Id)
                    .AsAsyncEnumerable();

                await foreach (var childId in childrenEnumerable)
                {
                    queue.Enqueue(childId);
                }
            }

            await _dbContext.Folders.Where(x => folderIds.Contains(x.Id))
                .ExecuteDeleteAsync(CancellationToken.None);

            // TODO: maybe this will just work without removing the related notes?
            //await _context.Notes.DeleteFromRelatedAsync(noteIds, CancellationToken.None);

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
