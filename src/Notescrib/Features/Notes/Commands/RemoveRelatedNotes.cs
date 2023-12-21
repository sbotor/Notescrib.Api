using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Data.MongoDb;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class RemoveRelatedNotes
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

            if (request.RelatedIds.Select(id => note.RelatedIds.Remove(id)).Any(removed => !removed))
            {
                throw new AppException(ErrorCodes.Note.RelatedNoteNotPresent);
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
                .WithErrorCode(ErrorCodes.Note.DuplicateRelatedNoteIds);
        }

        private static bool BeDistinct(IReadOnlyCollection<string> ids)
            => ids.Distinct().Count() == ids.Count;
    }
}
