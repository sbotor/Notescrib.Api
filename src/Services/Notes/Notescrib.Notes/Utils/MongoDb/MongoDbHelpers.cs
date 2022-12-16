using MongoDB.Driver;
using Notescrib.Notes.Features.Folders;

namespace Notescrib.Notes.Utils.MongoDb;

public static class MongoDbHelpers
{
    public static FilterDefinition<FolderData> GetNoteFilter(string noteId)
        => Builders<FolderData>.Filter.ElemMatch(x => x.Notes, x => x.Id == noteId);
}
