using FluentValidation;
using Notescrib.Contracts;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data.MongoDb;
using Notescrib.Features.Notes.Models;
using Notescrib.Features.Notes.Repositories;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Queries;

public static class GetNoteDetails
{
    public record Query(string Id) : IQuery<NoteDetails>;

    internal class Handler : IQueryHandler<Query, NoteDetails>
    {
        private readonly IMongoDbContext _context;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Note, NoteDetails> _mapper;

        public Handler(IMongoDbContext context, IPermissionGuard permissionGuard,
            IMapper<Note, NoteDetails> mapper)
        {
            _context = context;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
        }

        public async Task<NoteDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var include = new NoteIncludeOptions { Content = true, Related = true };
            var note = await _context.Notes.GetByIdAsync(request.Id, include, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            _permissionGuard.GuardCanView(note.OwnerId, note.SharingInfo);

            note.Related = note.Related
                .Where(x => _permissionGuard.CanView(x.OwnerId, x.SharingInfo))
                .OrderBy(x => x.Name.ToLowerInvariant())
                .ToArray();
            var details = _mapper.Map(note);

            return details;
        }
    }

    internal class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
