using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Utils;

namespace Notescrib.Features.Templates.Commands;

public static class CreateNoteTemplate
{
    public record Command(string Name) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IUserContext _userContext;
        private readonly IClock _clock;

        public Handler(NotescribDbContext dbContext, IUserContext userContext, IClock clock)
        {
            _dbContext = dbContext;
            _userContext = userContext;
            _clock = clock;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = await _userContext.GetUserId(CancellationToken.None);
            var workspace = await _dbContext.Workspaces.AsNoTracking()
                .FirstOrDefaultAsync(x => x.OwnerId == userId, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Workspace.WorkspaceNotFound);

            _dbContext.Add(new NoteTemplate
            {
                Name = request.Name,
                OwnerId = userId,
                WorkspaceId = workspace.Id,
                Created = _clock.Now,
                Content = new()
            });
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
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
