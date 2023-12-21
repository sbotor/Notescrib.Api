using Notescrib.Contracts;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data.MongoDb;
using Notescrib.Features.Templates.Models;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Templates.Queries;

public static class GetNoteTemplateDetails
{
    public record Query(string Id) : IQuery<NoteTemplateDetails>;

    internal class Handler : IQueryHandler<Query, NoteTemplateDetails>
    {
        private readonly IMongoDbContext _context;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<NoteTemplate, NoteTemplateDetails> _mapper;

        public Handler(IMongoDbContext context, IPermissionGuard permissionGuard, IMapper<NoteTemplate, NoteTemplateDetails> mapper)
        {
            _context = context;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
        }

        public async Task<NoteTemplateDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var template = await _context.NoteTemplates.GetByIdAsync(request.Id, cancellationToken);
            if (template == null)
            {
                throw new NotFoundException(ErrorCodes.NoteTemplate.NoteTemplateNotFound);
            }
            
            _permissionGuard.GuardCanView(template.OwnerId);

            return _mapper.Map(template);
        }
    }
}
