using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Extensions;
using Notescrib.Features.Notes.Mappers;
using Notescrib.Features.Notes.Models;
using Notescrib.Models;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Notes.Queries;

public static class SearchNotes
{
    public record Query(string? TextFilter, bool OwnOnly, Paging Paging) : IQuery<PagedList<NoteOverview>>;

    internal class Handler : IQueryHandler<Query, PagedList<NoteOverview>>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly INoteOverviewMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(
            NotescribDbContext dbContext,
            INoteOverviewMapper mapper,
            IUserContext userContext,
            IPermissionGuard permissionGuard)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContext = userContext;
            _permissionGuard = permissionGuard;
        }

        public async Task<PagedList<NoteOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = await _userContext.GetUserId(CancellationToken.None);

            var query = _dbContext.Notes.AsNoTracking()
                .Include(x => x.Tags)
                .Where(x => x.OwnerId == userId, request.OwnOnly);

            var (data, count) = await query.PaginateRaw(request.Paging, cancellationToken);

            var mapped = new List<NoteOverview>(data.Length);
            
            foreach (var item in data)
            {
                var isReadonly = !await _permissionGuard.CanEdit(item.OwnerId);
                mapped.Add(_mapper.Map(item, isReadonly));
            }

            return new(mapped, request.Paging.Page, request.Paging.PageSize, count);
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
