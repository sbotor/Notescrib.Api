using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Commands;

public static class RemoveRelatedNotes
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
            
            await _permissionGuard.GuardCanEdit(note.OwnerId);

            foreach (var id in request.RelatedIds)
            {
                var found = note.RelatedNotes.FirstOrDefault(x => x.RelatedId == id);
                if (found is null)
                {
                    throw new AppException(ErrorCodes.Note.RelatedNoteNotPresent);
                }

                note.RelatedNotes.Remove(found);
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
                .WithErrorCode(ErrorCodes.Note.DuplicateRelatedNoteIds);
        }

        private static bool BeDistinct(IReadOnlyCollection<Guid> ids)
            => ids.Distinct().Count() == ids.Count;
    }
}
