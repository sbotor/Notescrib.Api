using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Templates;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Models.Configuration;

namespace Notescrib.Notes.Utils.MongoDb;

public class MongoDbContext
{
    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;
        
        var db = new MongoClient(settings.ConnectionUri)
            .GetDatabase(settings.DatabaseName);
        
        Workspaces = db.GetCollection<Workspace>(settings.Collections.Workspaces);
        Folders = db.GetCollection<FolderData>(settings.Collections.Folders);
        Notes = db.GetCollection<NoteData>(settings.Collections.Notes);
        NoteTemplates = db.GetCollection<NoteTemplate>(settings.Collections.NoteTemplates);
    }
    
    public IMongoCollection<Workspace> Workspaces { get; }
    public IMongoCollection<FolderData> Folders { get; }
    public IMongoCollection<NoteData> Notes { get; }
    public IMongoCollection<NoteTemplate> NoteTemplates { get; }
}
