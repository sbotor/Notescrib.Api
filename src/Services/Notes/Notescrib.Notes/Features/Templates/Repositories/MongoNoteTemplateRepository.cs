using MongoDB.Driver;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Templates.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Templates.Repositories;

public class MongoNoteTemplateRepository : INoteTemplateRepository
{
    private readonly IMongoDbProvider _provider;
    private readonly SessionAccessor _sessionAccessor;

    public MongoNoteTemplateRepository(IMongoDbProvider provider, SessionAccessor sessionAccessor)
    {
        _provider = provider;
        _sessionAccessor = sessionAccessor;
    }

    public Task<NoteTemplate?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => _provider.NoteTemplates.SessionFind(_sessionAccessor.Session, x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken)!;

    public Task CreateAsync(NoteTemplate template, CancellationToken cancellationToken = default)
        => _provider.NoteTemplates.SessionInsertOneAsync(_sessionAccessor.Session, template,
            cancellationToken: cancellationToken);

    public Task UpdateAsync(NoteTemplate template, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteTemplate>.Update
            .Set(x => x.Name, template.Name)
            .Set(x => x.Updated, template.Updated);

        return _provider.NoteTemplates.SessionUpdateOneAsync(_sessionAccessor.Session, x => x.Id == template.Id, update,
            cancellationToken: cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => _provider.NoteTemplates.SessionDeleteOneAsync(_sessionAccessor.Session, x => x.Id == id, cancellationToken);

    public Task DeleteAllAsync(string workspaceId, CancellationToken cancellationToken = default)
        => _provider.NoteTemplates.SessionDeleteManyAsync(_sessionAccessor.Session, x => x.WorkspaceId == workspaceId, cancellationToken);

    public Task UpdateContentAsync(NoteTemplate template, CancellationToken cancellationToken = default)
    {
        var update = Builders<NoteTemplate>.Update
            .Set(x => x.Content, template.Content)
            .Set(x => x.Updated, template.Updated);

        return _provider.NoteTemplates.SessionUpdateOneAsync(_sessionAccessor.Session,x => x.Id == template.Id, update,
            cancellationToken: cancellationToken);
    }

    public Task<PagedList<NoteTemplate>> SearchAsync(string ownerId, string? textFilter,
        PagingSortingInfo<NoteTemplatesSorting> pagingSortingInfo,
        CancellationToken cancellationToken)
    {
        var filters = new List<ExpressionFilterDefinition<NoteTemplate>> { new(x => x.OwnerId == ownerId) };
        if (!string.IsNullOrEmpty(textFilter))
        {
            filters.Add(new(x => x.Name.Contains(textFilter)));
        }

        ProjectionDefinition<NoteTemplate, NoteTemplate> projection = Builders<NoteTemplate>.Projection
            .Exclude(x => x.Content);

        return _provider.NoteTemplates.FindPagedAsync(_sessionAccessor.Session, filters, pagingSortingInfo, projection, cancellationToken);
    }
}
