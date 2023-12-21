using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data.MongoDb;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Templates.Commands;

public static class DeleteNoteTemplate
{
    public record Command(string Id) : ICommand;

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

            await _context.NoteTemplates.DeleteAsync(template.Id, CancellationToken.None);

            return Unit.Value;
        }
    }
}
