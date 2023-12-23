using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Templates.Commands;

public static class UpdateNoteTemplate
{
    public record Command(Guid Id, string Name) : ICommand;

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
            var template = await _dbContext.NoteTemplates
                .FirstOrDefaultAsync(x => x.Id == request.Id, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.NoteTemplate.NoteTemplateNotFound);
            
            await _permissionGuard.GuardCanEdit(template.OwnerId);

            template.Name = request.Name;

            await _dbContext.SaveChangesAsync(CancellationToken.None);

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
