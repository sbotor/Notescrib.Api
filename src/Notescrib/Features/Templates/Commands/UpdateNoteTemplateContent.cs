using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Templates.Commands;

public static class UpdateNoteTemplateContent
{
    public record Command(Guid Id, string Content) : ICommand;
    
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
            var template = await _dbContext.NoteTemplates
                .FirstOrDefaultAsync(x => x.Id == request.Id, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.NoteTemplate.NoteTemplateNotFound);
            
            await _permissionGuard.GuardCanEdit(template.OwnerId);

            template.Content = new() { Content = request.Content };
            template.Updated = _clock.Now;

            await _dbContext.SaveChangesAsync(CancellationToken.None);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();

            RuleFor(x => x.Content)
                .MaximumLength(Consts.NoteTemplate.MaxContentLength);
        }
    }
}
