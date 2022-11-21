using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Utils.MongoDb;

public static class MongoDbClassMaps
{
    private static readonly IIdGenerator IdGenerator = StringObjectIdGenerator.Instance;
    
    private static bool Registered;
    
    public static void Register()
    {
        if (Registered)
        {
            return;
        }
        
        BsonClassMap.RegisterClassMap<Workspace>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(x => x.Id).SetIdGenerator(IdGenerator);
        });

        BsonClassMap.RegisterClassMap<Note>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(x => x.Id).SetIdGenerator(IdGenerator);
        });

        Registered = true;
    }
}
