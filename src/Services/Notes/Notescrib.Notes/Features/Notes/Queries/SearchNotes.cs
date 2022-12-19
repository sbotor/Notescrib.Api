using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Queries;

public static class SearchNotes
{
    public record Query(string? TextFilter, bool OwnOnly) : IQuery<PagedList<NoteOverview>>;

    internal class Handler : IQueryHandler<Query, PagedList<NoteOverview>>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IMapper<NoteData, NoteOverview> _mapper;
        private readonly IUserContextProvider _userContextProvider;

        public Handler(IFolderRepository folderRepository, IMapper<NoteData, NoteOverview> mapper, IUserContextProvider userContextProvider)
        {
            _folderRepository = folderRepository;
            _mapper = mapper;
            _userContextProvider = userContextProvider;
        }

        public Task<PagedList<NoteOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new PagedList<NoteOverview>(Array.Empty<NoteOverview>(), 1, 0, 0));
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
