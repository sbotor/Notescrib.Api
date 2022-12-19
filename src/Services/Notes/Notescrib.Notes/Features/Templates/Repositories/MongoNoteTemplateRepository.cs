﻿using MongoDB.Driver;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Templates.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Templates.Repositories;

public class MongoNoteTemplateRepository : INoteTemplateRepository
{
    private readonly MongoDbContext _context;

    public MongoNoteTemplateRepository(MongoDbContext context)
    {
        _context = context;
    }

    public Task<NoteTemplate?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        => _context.NoteTemplates.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken)!;

    public Task CreateAsync(NoteTemplate template, CancellationToken cancellationToken = default)
        => _context.NoteTemplates.InsertOneAsync(template, cancellationToken: cancellationToken);

    public Task UpdateAsync(NoteTemplateBase template, CancellationToken cancellationToken = default)
        => UpdateCore(template.Id, GetUpdateDefBase(template), cancellationToken);

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default)
        => _context.NoteTemplates.DeleteOneAsync(x => x.Id == id, cancellationToken);

    public Task UpdateWithContentAsync(NoteTemplate template, CancellationToken cancellationToken = default)
    {
        var update = GetUpdateDefBase(template)
            .Set(x => x.Content, template.Content);

        return UpdateCore(template.Id, update, cancellationToken);
    }

    public Task<PagedList<NoteTemplateBase>> SearchAsync(string ownerId, string? textFilter, PagingSortingInfo<NoteTemplatesSorting> pagingSortingInfo,
        CancellationToken cancellationToken)
    {
        var filters = new List<ExpressionFilterDefinition<NoteTemplate>> { new(x => x.OwnerId == ownerId) };
        if (!string.IsNullOrEmpty(textFilter))
        {
            filters.Add(new(x => x.Name.Contains(textFilter)));
        }

        ProjectionDefinition<NoteTemplate, NoteTemplateBase> projection = Builders<NoteTemplate>.Projection
            .Exclude(x => x.Content);

        return _context.NoteTemplates.FindPagedAsync(filters, pagingSortingInfo, projection, cancellationToken);
    }

    private Task UpdateCore(string id, UpdateDefinition<NoteTemplate> update, CancellationToken cancellationToken)
        => _context.NoteTemplates.UpdateOneAsync(x => x.Id == id, update,
            cancellationToken: cancellationToken);
    
    private static UpdateDefinition<NoteTemplate> GetUpdateDefBase(NoteTemplateBase template)
        => Builders<NoteTemplate>.Update
            .Set(x => x.Name, template.Name)
            .Set(x => x.Updated, template.Updated);
}