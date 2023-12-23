using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data;
using Notescrib.Features.Notes.Mappers;
using Notescrib.Features.Notes.Models;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Queries;

public static class GetNoteDetails
{
    public record Query(Guid Id) : IQuery<NoteDetails>;

    internal class Handler : IQueryHandler<Query, NoteDetails>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IPermissionGuard _permissionGuard;
        private readonly INoteDetailsMapper _detailsMapper;
        private readonly INoteOverviewMapper _overviewMapper;

        public Handler(NotescribDbContext dbContext, IPermissionGuard permissionGuard,
            INoteDetailsMapper detailsMapper,
            INoteOverviewMapper overviewMapper)
        {
            _dbContext = dbContext;
            _permissionGuard = permissionGuard;
            _detailsMapper = detailsMapper;
            _overviewMapper = overviewMapper;
        }

        public async Task<NoteDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var note = await _dbContext.Notes.AsNoTracking()
                .Include(x => x.RelatedNotes)
                .Include(x => x.Tags)
                .Include(x => x.Content)
                .FirstOrDefaultAsync(x => x.Id == request.Id, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Note.NoteNotFound);

            await _permissionGuard.GuardCanView(note.OwnerId, note.GetSharingInfo());

            var relatedIds = note.RelatedNotes.Select(x => x.RelatedId).ToArray();

            var relatedNotes = relatedIds.Length < 1
                ? Array.Empty<Note>()
                : await _dbContext.Notes.AsNoTracking()
                    .Include(x => x.Tags)
                    .Where(x => relatedIds.Contains(x.Id))
                    .ToArrayAsync(CancellationToken.None);
            
            var details = _detailsMapper.Map(note, !await _permissionGuard.CanEdit(note.OwnerId));

            var relatedOverviews = new List<NoteOverview>();
            foreach (var related in relatedNotes)
            {
                if (!await _permissionGuard.CanView(related.OwnerId, related.GetSharingInfo()))
                {
                    continue;
                }

                var canEdit = await _permissionGuard.CanEdit(related.OwnerId);
                relatedOverviews.Add(_overviewMapper.Map(related, !canEdit));
            }

            details.Related = relatedOverviews;

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
