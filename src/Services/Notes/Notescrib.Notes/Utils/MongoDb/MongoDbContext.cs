﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Notes;
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

        Notes = db.GetCollection<NoteBase>(settings.Collections.Notes);
        Workspaces = db.GetCollection<Workspace>(settings.Collections.Workspaces);
        Folders = db.GetCollection<FolderBase>(settings.Collections.Folders);
    }
    
    public IMongoCollection<NoteBase> Notes { get; }
    public IMongoCollection<Workspace> Workspaces { get; }
    public IMongoCollection<FolderBase> Folders { get; }
}
