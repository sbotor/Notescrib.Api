using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data.MongoDb;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Commands;

public static class AddRelatedNotes
{
    public class Command : ICommand
    {
        public string Id { get; }
        public IReadOnlyCollection<string> RelatedIds { get; }

        public Command(string id, IEnumerable<string> relatedIds)
        {
            Id = id;
            RelatedIds = relatedIds.ToArray();
        }
    }

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
            var note = await _context.Notes.GetByIdAsync(request.Id, new() { Related = true }, CancellationToken.None);
            if (note == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            if (note.Related.Count >= Consts.Note.MaxRelatedCount)
            {
                throw new AppException(ErrorCodes.Note.MaximumRelatedNoteCountReached);
            }

            var foundNotes = await _context.Notes.GetManyAsync(request.RelatedIds, CancellationToken.None);

            if (foundNotes.Any(x => !_permissionGuard.CanEdit(x.OwnerId)))
            {
                throw new ForbiddenException();
            }

            if (foundNotes.Count != request.RelatedIds.Count)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            foreach (var id in request.RelatedIds)
            {
                if (note.Related.Any(x => x.Id == id))
                {
                    throw new AppException(ErrorCodes.Note.DuplicateRelatedNoteIds);
                }

                note.RelatedIds.Add(id);
            }

            await _context.Notes.UpdateRelatedAsync(note, CancellationToken.None);

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

        private static bool BeDistinct(IReadOnlyCollection<string> ids)
            => ids.Distinct().Count() == ids.Count;

        private static bool NotIncludeParent(Command command, IEnumerable<string> ids)
            => ids.All(x => x != command.Id);
    }
}
