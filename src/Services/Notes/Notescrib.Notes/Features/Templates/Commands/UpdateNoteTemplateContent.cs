using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Templates.Commands;

public class UpdateNoteTemplateContent
{
    public record Command(string Id, string Content) : ICommand;
    
    internal class Handler : ICommandHandler<Command>
    {
        private readonly MongoDbContext _context;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(MongoDbContext context, IPermissionGuard permissionGuard, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var template = await _context.NoteTemplates.GetByIdAsync(request.Id, cancellationToken);
            if (template == null)
            {
                throw new NotFoundException(ErrorCodes.NoteTemplate.NoteTemplateNotFound);
            }
            
            _permissionGuard.GuardCanEdit(template.OwnerId);

            template.Content = request.Content;
            template.Updated = _dateTimeProvider.Now;
            
            await _context.NoteTemplates.UpdateContentAsync(template, CancellationToken.None);

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
