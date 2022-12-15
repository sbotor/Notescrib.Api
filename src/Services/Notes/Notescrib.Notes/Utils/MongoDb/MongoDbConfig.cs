﻿using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Templates;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Utils.MongoDb;

public static class MongoDbConfig
{
    private static readonly IIdGenerator IdGenerator = StringObjectIdGenerator.Instance;

    public static void RegisterClassMaps()
    {
        BsonClassMap.RegisterClassMap<Workspace>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(x => x.Id).SetIdGenerator(IdGenerator);
        });
        
        BsonClassMap.RegisterClassMap<FolderData>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(x => x.Id).SetIdGenerator(IdGenerator);
        });
        
        BsonClassMap.RegisterClassMap<NoteContentData>(cm =>
        {
            cm.AutoMap();
        });

        BsonClassMap.RegisterClassMap<NoteTemplateBase>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(x => x.Id).SetIdGenerator(IdGenerator);
        });
    }
}