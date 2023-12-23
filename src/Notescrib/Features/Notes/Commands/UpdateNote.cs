using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Models;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Commands;

public static class UpdateNote
{
    public record Command(
            Guid Id,
            string Name,
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
            var note = await _dbContext.Notes.Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == request.Id, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Note.NoteNotFound);

            await _permissionGuard.GuardCanEdit(note.OwnerId);

            var folder = await _dbContext.Folders.AsNoTracking()
                .Include(x => x.Notes)
                .FirstAsync(x => x.Id == note.FolderId, CancellationToken.None);

            if (note.Name != request.Name && folder.Notes.Any(x => x.Name == request.Name))
            {
                throw new DuplicationException(ErrorCodes.Note.NoteAlreadyExists);
            }

            note.Name = request.Name;
            note.Visibility = request.SharingInfo.Visibility;
            note.Updated = _clock.Now;
            
            UpdateTags(note, request.Tags);

            await _dbContext.SaveChangesAsync(CancellationToken.None);

            return Unit.Value;
        }

        private static void UpdateTags(Note note, IEnumerable<string> tags)
        {
            var newTags = tags.ToList();
            
            foreach (var tag in note.Tags.ToArray())
            {
                var keep = newTags.Remove(tag.Value);
                if (!keep)
                {
                    note.Tags.Remove(tag);
                }
            }

            foreach (var tag in newTags)
            {
                note.Tags.Add(new() { Value = tag });
            }
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Consts.Name.MaxLength);

            RuleFor(x => x.Tags.Count)
                .LessThanOrEqualTo(Consts.Note.MaxTagCount);
            RuleForEach(x => x.Tags)
                .NotEmpty();

            RuleFor(x => x.SharingInfo)
                .NotNull();
        }
    }
}
