using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Features.Folders.Extensions;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Folders.Commands;

public static class CreateFolder
{
    public record Command(string Name, Guid? ParentId) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IClock _clock;

        public Handler(
            NotescribDbContext dbContext,
            IPermissionGuard permissionGuard,
            IClock clock)
        {
            _dbContext = dbContext;
            _permissionGuard = permissionGuard;
            _clock = clock;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = await _permissionGuard.UserContext.GetUserId(CancellationToken.None);
            var now = _clock.Now;
            
            var parent = await _dbContext.Folders.AsNoTracking()
                .GetFolderOrRootAsync(userId, request.ParentId, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);

            await _permissionGuard.GuardCanEdit(parent.OwnerId);
            
            if (parent.NestingLevel >= Consts.Folder.MaxNestingLevel)
            {
                throw new AppException(ErrorCodes.Folder.CannotNestMoreChildren);
            }

            var children = await _dbContext.Folders.AsNoTracking()
                .Where(x => x.ParentId == parent.Id)
                .ToArrayAsync(CancellationToken.None);
            
            if (children.Length >= Consts.Folder.MaxChildrenCount)
            {
                throw new AppException(ErrorCodes.Folder.MaximumFolderCountReached);
            }

            if (children.Any(x => x.Name == request.Name))
            {
                throw new DuplicationException(ErrorCodes.Folder.FolderAlreadyExists);
            }
            
            var folder = new Folder
            {
                Name = request.Name,
                Created = now,
                OwnerId = userId,
                ParentId = parent.Id,
                WorkspaceId = parent.WorkspaceId,
                NestingLevel = parent.NestingLevel + 1
            };

            _dbContext.Add(folder);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

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
