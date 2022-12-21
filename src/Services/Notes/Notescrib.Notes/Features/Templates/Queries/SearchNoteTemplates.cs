using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Services;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Templates.Models;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Features.Templates.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Templates.Queries;

public static class SearchNoteTemplates
{
    public record Query(string? TextFilter, Paging Paging) : IQuery<PagedList<NoteTemplateOverview>>;

    internal class Handler : IQueryHandler<Query, PagedList<NoteTemplateOverview>>
    {
        private readonly INoteTemplateRepository _repository;
        private readonly IUserContextProvider _userContextProvider;
        private readonly IMapper<NoteTemplateBase, NoteTemplateOverview> _mapper;
        private readonly ISortingProvider<NoteTemplatesSorting> _sortingProvider;

        public Handler(INoteTemplateRepository repository, IUserContextProvider userContextProvider,
            IMapper<NoteTemplateBase, NoteTemplateOverview> mapper,
            ISortingProvider<NoteTemplatesSorting> sortingProvider)
        {
            _repository = repository;
            _userContextProvider = userContextProvider;
            _mapper = mapper;
            _sortingProvider = sortingProvider;
        }

        public async Task<PagedList<NoteTemplateOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var sorting = new Sorting<NoteTemplatesSorting>();
            var info = new PagingSortingInfo<NoteTemplatesSorting>(request.Paging, sorting, _sortingProvider);

            var templates = await _repository.SearchAsync(_userContextProvider.UserId, request.TextFilter, info,
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
