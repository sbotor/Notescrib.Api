using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Folders.Commands;

public static class UpdateFolder
{
    public record Command(Guid FolderId, string Name) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IClock _clock;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(
            NotescribDbContext dbContext,
            IClock clock,
            IPermissionGuard permissionGuard)
        {
            _dbContext = dbContext;
            _clock = clock;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _dbContext.Folders
                .FirstOrDefaultAsync(x => x.Id == request.FolderId, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);
            
            await _permissionGuard.GuardCanEdit(folder.OwnerId);

            if (folder.ParentId == null)
            {
                throw new AppException("Cannot update root folder.");
            }
            
            var children = await _dbContext.Folders.AsNoTracking()
                .Where(x => x.ParentId == folder.ParentId)
                .ToArrayAsync(CancellationToken.None);

            if (children.Any(x => x.Name == request.Name))
            {
                throw new DuplicationException(ErrorCodes.Folder.FolderAlreadyExists);
            }

            var now = _clock.Now;
            folder.Updated = now;
            folder.Name = request.Name;

            await _dbContext.SaveChangesAsync(CancellationToken.None);

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
