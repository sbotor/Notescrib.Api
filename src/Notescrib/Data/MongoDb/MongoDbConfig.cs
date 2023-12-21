using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using Notescrib.Features.Folders;
using Notescrib.Features.Notes;
using Notescrib.Features.Templates;
using Notescrib.Features.Workspaces;

namespace Notescrib.Data.MongoDb;

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

        BsonClassMap.RegisterClassMap<NoteTemplate>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(x => x.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
        });
    }
}
