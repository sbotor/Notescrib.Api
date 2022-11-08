using MongoDB.Driver;
using Notescrib.Notes.Application.Models.Configuration;

namespace Notescrib.Notes.Application.Services;

public interface IMongoCollectionProvider
{
    IMongoCollection<TDocument> GetCollection<TDocument>(string name) where TDocument : class;
    CollectionNames CollectionNames { get; }
}
