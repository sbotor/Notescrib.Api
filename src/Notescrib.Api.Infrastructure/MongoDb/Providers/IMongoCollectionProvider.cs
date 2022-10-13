using MongoDB.Driver;

namespace Notescrib.Api.Infrastructure.MongoDb.Providers;
internal interface IMongoCollectionProvider
{
    IMongoCollection<TDocument> GetCollection<TDocument>() where TDocument : class;
}
