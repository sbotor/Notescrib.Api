using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Commands;

public static class UpdateNoteContent
{
    public record Command(Guid NoteId, string Content) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IClock _clock;

        public Handler(NotescribDbContext dbContext, IPermissionGuard permissionGuard, IClock clock)
        {
            _dbContext = dbContext;
            _permissionGuard = permissionGuard;
            _clock = clock;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _dbContext.Notes
                .FirstOrDefaultAsync(x => x.Id == request.NoteId, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            
            await _permissionGuard.GuardCanEdit(note.OwnerId);

            note.Content = new() { Content = request.Content };
            note.Updated = _clock.Now;

            await _dbContext.SaveChangesAsync(CancellationToken.None);

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
