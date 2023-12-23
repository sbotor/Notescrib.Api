using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Notescrib.Contracts;
using Notescrib.Core.Cqrs;
using Notescrib.Data;
using Notescrib.Extensions;
using Notescrib.Features.Templates.Models;
using Notescrib.Models;
using Notescrib.Utils;

namespace Notescrib.Features.Templates.Queries;

public static class SearchNoteTemplates
{
    public record Query(string? TextFilter, Paging Paging) : IQuery<PagedList<NoteTemplateOverview>>;

    internal class Handler : IQueryHandler<Query, PagedList<NoteTemplateOverview>>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IMapper<NoteTemplate, NoteTemplateOverview> _mapper;

        public Handler(NotescribDbContext dbContext,
            IMapper<NoteTemplate, NoteTemplateOverview> mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public Task<PagedList<NoteTemplateOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _dbContext.NoteTemplates.AsNoTracking()
                .Where(x => x.Name == request.TextFilter, !string.IsNullOrEmpty(request.TextFilter));

            return query.Paginate(request.Paging, _mapper.Map, cancellationToken);
        }
    }

    internal class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.TextFilter)
                .MaximumLength(Consts.Name.MaxLength)
                .When(x => string.IsNullOrEmpty(x.TextFilter));

            RuleFor(x => x.Paging)
                .SetValidator(new Paging.Validator());
        }
    }
}
