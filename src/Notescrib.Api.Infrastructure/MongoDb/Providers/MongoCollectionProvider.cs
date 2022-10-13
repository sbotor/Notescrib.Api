using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Infrastructure.MongoDb.Providers;

internal class MongoCollectionProvider : IMongoCollectionProvider
{
    // TODO: This should probably be customizable.
    private readonly Dictionary<Type, string> _collectionMap = new Dictionary<Type, string>
    {
        [typeof(Workspace)] = "Workspaces",
        [typeof(Note)] = "Notes"
    };

    private readonly MongoDbSettings _settings;
    private readonly IMongoClient _client;
    private readonly IMongoDatabase _database;

    public MongoCollectionProvider(IOptions<MongoDbSettings> options)
    {
        _settings = options.Value;
        _client = new MongoClient(_settings.ConnectionUri);
        _database = _client.GetDatabase(_settings.DatabaseName);
    }

    public IMongoCollection<TDocument> GetCollection<TDocument>() where TDocument : class
    {
        if (!_collectionMap.TryGetValue(typeof(TDocument), out var collectionName))
        {
            throw new InvalidOperationException($"No collection name for type {typeof(TDocument).Name}.");
        }

        return _database.GetCollection<TDocument>(collectionName);
    }
}
