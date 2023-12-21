using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data.MongoDb;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Templates.Commands;

public static class UpdateNoteTemplate
{
    public record Command(string Id, string Name) : ICommand;

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
            var template = await _context.NoteTemplates.GetByIdAsync(request.Id, cancellationToken);
            if (template == null)
            {
                throw new NotFoundException(ErrorCodes.NoteTemplate.NoteTemplateNotFound);
            }
            
            _permissionGuard.GuardCanEdit(template.OwnerId);

            template.Name = request.Name;
            
            await _context.NoteTemplates.UpdateAsync(template, CancellationToken.None);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .MaximumLength(Consts.Name.MaxLength);

            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
