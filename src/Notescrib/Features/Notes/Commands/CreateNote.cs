using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Features.Folders.Extensions;
using Notescrib.Models;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Commands;

public static class CreateNote
{
    public record Command(
            string Name,
            Guid? FolderId,
            string? Content,
            IReadOnlyCollection<string> Tags,
            SharingInfo SharingInfo)
        : ICommand;

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
            
            var folder = await _dbContext.Folders.Include(x => x.Notes)
                .GetFolderOrRootAsync(userId, request.FolderId, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);
            
            await _permissionGuard.GuardCanEdit(folder.OwnerId);
            
            if (folder.Notes.Count >= Consts.Folder.MaxNoteCount)
            {
                throw new AppException(ErrorCodes.Folder.MaximumNoteCountReached);
            }
            
            if (folder.Notes.Any(x => x.Name == request.Name))
            {
                throw new AppException(ErrorCodes.Note.NoteAlreadyExists);
            }

            var now = _clock.Now;
            var note = new Note
            {
                Name = request.Name,
                OwnerId = userId,
                Visibility = request.SharingInfo.Visibility,
                Created = now,
                WorkspaceId = folder.WorkspaceId,
                FolderId = folder.Id,
                Content = new NoteContent { Content = request.Content ?? string.Empty },
                Tags = request.Tags.Select(x => new NoteTag { Value = x }).ToArray()
            };

            _dbContext.Add(note);
            
            folder.Updated = now;

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

            RuleFor(x => x.Tags.Count)
                .LessThanOrEqualTo(Consts.Note.MaxTagCount);
            RuleForEach(x => x.Tags)
                .NotEmpty();

            RuleFor(x => x.SharingInfo)
                .NotNull();

            RuleFor(x => x.Content)
                .MaximumLength(Consts.Note.MaxContentLength)
                .When(x => !string.IsNullOrEmpty(x.Content));
        }
    }
}
