using FluentValidation;
using Notescrib.Contracts;
using Notescrib.Core.Cqrs;
using Notescrib.Data.MongoDb;
using Notescrib.Features.Templates.Models;
using Notescrib.Features.Templates.Utils;
using Notescrib.Models;
using Notescrib.Utils;

namespace Notescrib.Features.Templates.Queries;

public static class SearchNoteTemplates
{
    public record Query(string? TextFilter, Paging Paging) : IQuery<PagedList<NoteTemplateOverview>>;

    internal class Handler : IQueryHandler<Query, PagedList<NoteTemplateOverview>>
    {
        private readonly IMongoDbContext _context;
        private readonly IMapper<NoteTemplate, NoteTemplateOverview> _mapper;
        private readonly ISortingProvider<NoteTemplatesSorting> _sortingProvider;

        public Handler(IMongoDbContext context,
            IMapper<NoteTemplate, NoteTemplateOverview> mapper,
            ISortingProvider<NoteTemplatesSorting> sortingProvider)
        {
            _context = context;
            _mapper = mapper;
            _sortingProvider = sortingProvider;
        }

        public async Task<PagedList<NoteTemplateOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var sorting = new Sorting<NoteTemplatesSorting>();
            var info = new PagingSortingInfo<NoteTemplatesSorting>(request.Paging, sorting, _sortingProvider);

            var templates = await _context.NoteTemplates.SearchAsync(
                request.TextFilter,
                info,
                cancellationToken);

            return templates.Map(_mapper.Map);
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
