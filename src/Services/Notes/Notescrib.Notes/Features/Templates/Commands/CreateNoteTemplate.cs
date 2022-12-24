using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Templates.Commands;

public static class CreateNoteTemplate
{
    public record Command(string Name) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly MongoDbContext _context;
        private readonly IUserContextProvider _userContextProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(MongoDbContext context, IUserContextProvider userContextProvider, IDateTimeProvider dateTimeProvider)
        {
            _context = context;
            _userContextProvider = userContextProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var workspace = await _context.Workspaces.GetByOwnerIdAsync(cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException(ErrorCodes.Workspace.WorkspaceNotFound);
            }

            var template = new NoteTemplate
            {
                Name = request.Name,
                OwnerId = _userContextProvider.UserId,
                WorkspaceId = workspace.Id,
                Created = _dateTimeProvider.Now,
                Content = string.Empty
            };

            await _context.NoteTemplates.CreateAsync(template, cancellationToken);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Consts.Name.MaxLength);
        }
    }
}
