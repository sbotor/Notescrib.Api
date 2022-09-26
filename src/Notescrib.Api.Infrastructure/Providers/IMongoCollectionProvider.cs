using MongoDB.Driver;

namespace Notescrib.Api.Infrastructure.Services;
internal interface IMongoCollectionProvider
{
    IMongoCollection<TDocument> GetCollection<TDocument>() where TDocument : class;
}
