using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data.MongoDb;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Commands;

public static class UpdateNoteContent
{
    public record Command(string NoteId, string Content) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IMongoDbContext _context;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(IMongoDbContext context, IPermissionGuard permissionGuard, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _context.Notes.GetByIdAsync(request.NoteId, cancellationToken: cancellationToken);
            if (note == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }
            
            _permissionGuard.GuardCanEdit(note.OwnerId);

            note.Content = request.Content;
            note.Updated = _dateTimeProvider.Now;
            
            await _context.Notes.UpdateContentAsync(note, CancellationToken.None);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.NoteId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .MaximumLength(Consts.Note.MaxContentLength);
        }
    }
}
