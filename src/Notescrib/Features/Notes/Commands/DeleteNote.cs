using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data.MongoDb;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Commands;

public static class DeleteNote
{
    public record Command(string Id) : ICommand;

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
            var note = await _context.Notes.GetByIdAsync(request.Id, new() { Folder = true }, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            _permissionGuard.GuardCanEdit(note.OwnerId);

            note.Folder.Updated = _dateTimeProvider.Now;

            await _context.EnsureTransactionAsync(CancellationToken.None);
            
            await _context.Folders.UpdateAsync(note.Folder, CancellationToken.None);

            await _context.Notes.DeleteFromRelatedAsync(note.Id, CancellationToken.None);
            await _context.Notes.DeleteAsync(note.Id, CancellationToken.None);

            await _context.CommitTransactionAsync();

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
