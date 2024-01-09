using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Commands;

public static class AddRelatedNotes
{
    public class Command : ICommand
    {
        public Guid Id { get; }
        public IReadOnlyCollection<Guid> RelatedIds { get; }

        public Command(Guid id, IEnumerable<Guid> relatedIds)
        {
            Id = id;
            RelatedIds = relatedIds.ToArray();
        }
    }

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
            var note = await _dbContext.Notes.Include(x => x.RelatedNotes)
                .Where(x => x.Id == request.Id)
                .FirstOrDefaultAsync(CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Note.NoteNotFound);

            if (note.RelatedNotes.Count >= Consts.Note.MaxRelatedCount)
            {
                throw new AppException(ErrorCodes.Note.MaximumRelatedNoteCountReached);
            }

            var relatedOwnerIds = await _dbContext.Notes.AsNoTracking()
                .Where(x => request.RelatedIds.Contains(x.Id))
                .Select(x => x.OwnerId)
                .ToArrayAsync(CancellationToken.None);

            foreach (var relatedOwnerId in relatedOwnerIds)
            {
                if (!await _permissionGuard.CanEdit(relatedOwnerId))
                {
                    throw new ForbiddenException();
                }
            }

            if (relatedOwnerIds.Length < request.RelatedIds.Count)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            foreach (var id in request.RelatedIds)
            {
                if (note.RelatedNotes.Any(x => x.RelatedId == id))
                {
                    throw new AppException(ErrorCodes.Note.DuplicateRelatedNoteIds);
                }

                note.RelatedNotes.Add(new() { RelatedId = id, NoteId = note.Id });
            }

            await _dbContext.SaveChangesAsync(CancellationToken.None);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.RelatedIds.Count)
                .NotEmpty()
                .LessThanOrEqualTo(Consts.Note.MaxRelatedCount);

            RuleFor(x => x.RelatedIds)
                .Must(BeDistinct)
                .WithErrorCode(ErrorCodes.Note.DuplicateRelatedNoteIds)
                .Must(NotIncludeParent)
                .WithErrorCode(ErrorCodes.Note.InvalidRelatedNoteId);
        }

        private static bool BeDistinct(IReadOnlyCollection<Guid> ids)
            => ids.Distinct().Count() == ids.Count;

        private static bool NotIncludeParent(Command command, IEnumerable<Guid> ids)
            => ids.All(x => x != command.Id);
    }
}
