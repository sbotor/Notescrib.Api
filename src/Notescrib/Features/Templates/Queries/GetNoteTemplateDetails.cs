using Microsoft.EntityFrameworkCore;
using Notescrib.Contracts;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data;
using Notescrib.Features.Templates.Models;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Templates.Queries;

public static class GetNoteTemplateDetails
{
    public record Query(Guid Id) : IQuery<NoteTemplateDetails>;

    internal class Handler : IQueryHandler<Query, NoteTemplateDetails>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<NoteTemplate, NoteTemplateDetails> _mapper;

        public Handler(NotescribDbContext dbContext, IPermissionGuard permissionGuard, IMapper<NoteTemplate, NoteTemplateDetails> mapper)
        {
            _dbContext = dbContext;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
        }

        public async Task<NoteTemplateDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var template = await _dbContext.NoteTemplates.AsNoTracking()
                .Include(x => x.Content)
                .FirstOrDefaultAsync(x => x.Id == request.Id, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.NoteTemplate.NoteTemplateNotFound);
            
            await _permissionGuard.GuardCanView(template.OwnerId);

            return _mapper.Map(template);
        }
    }
}
