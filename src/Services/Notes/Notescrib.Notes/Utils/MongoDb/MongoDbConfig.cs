using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Templates;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Utils.MongoDb;

public static class MongoDbConfig
{
    public static void RegisterClassMaps()
    {
        BsonClassMap.RegisterClassMap<Workspace>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
        });
        
        BsonClassMap.RegisterClassMap<FolderData>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
        });
        
        BsonClassMap.RegisterClassMap<NoteBase>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
        });

        BsonClassMap.RegisterClassMap<NoteTemplateBase>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
        });
    }
}
