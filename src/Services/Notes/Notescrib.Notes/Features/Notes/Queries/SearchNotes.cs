using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Services;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Notes.Queries;

public static class SearchNotes
{
    public record Query(string? TextFilter, bool OwnOnly, Paging Paging) : IQuery<PagedList<NoteOverview>>;

    internal class Handler : IQueryHandler<Query, PagedList<NoteOverview>>
    {
        private readonly MongoDbContext _context;
        private readonly IMapper<NoteBase, NoteOverview> _mapper;
        private readonly IUserContextProvider _userContextProvider;
        private readonly ISortingProvider<NotesSorting> _sortingProvider;

        public Handler(MongoDbContext context, IMapper<NoteBase, NoteOverview> mapper,
            IUserContextProvider userContextProvider, ISortingProvider<NotesSorting> sortingProvider)
        {
            _context = context;
            _mapper = mapper;
            _userContextProvider = userContextProvider;
            _sortingProvider = sortingProvider;
        }

        public async Task<PagedList<NoteOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var info = new PagingSortingInfo<NotesSorting>(request.Paging, new(), _sortingProvider);

            var result = await _context.Notes.SearchAsync(
                _userContextProvider.UserIdOrDefault,
                request.TextFilter,
                request.OwnOnly,
                info,
                cancellationToken);

            return result.Map(_mapper.Map);
        }
    }

    internal class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.TextFilter)
                .MaximumLength(Consts.Name.MaxLength)
                .When(x => !string.IsNullOrEmpty(x.TextFilter));
        }
    }
}
