using MongoDB.Driver;
using Notescrib.Notes.Models.Configuration;

namespace Notescrib.Notes.Services;

public interface IMongoCollectionProvider
{
    IMongoCollection<TDocument> GetCollection<TDocument>(string name) where TDocument : class;
    CollectionNames CollectionNames { get; }
}
