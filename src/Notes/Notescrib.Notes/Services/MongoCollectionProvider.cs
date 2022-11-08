using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Notescrib.Notes.Models.Configuration;

namespace Notescrib.Notes.Services;

public class MongoCollectionProvider : IMongoCollectionProvider
{
    private readonly MongoDbSettings _settings;
    private readonly IMongoDatabase _database;

    public CollectionNames CollectionNames => _settings.CollectionNames;

    public MongoCollectionProvider(IOptions<MongoDbSettings> options)
    {
        _settings = options.Value;
        
        _database = new MongoClient(_settings.ConnectionUri)
            .GetDatabase(_settings.DatabaseName);
    }

    public IMongoCollection<TDocument> GetCollection<TDocument>(string name) where TDocument : class
        => _database.GetCollection<TDocument>(name,
            new() { AssignIdOnInsert = true });
}
