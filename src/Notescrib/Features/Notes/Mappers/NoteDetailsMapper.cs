using Notescrib.Contracts;
using Notescrib.Features.Notes.Models;
using Notescrib.Services;

namespace Notescrib.Features.Notes.Mappers;

public interface INoteDetailsMapper
{
    NoteDetails Map(Note item, bool isReadonly);
}

public class NoteDetailsMapper : INoteDetailsMapper
{
    public NoteDetails Map(Note item, bool isReadonly)
        => new()
        {
            Id = item.Id,
            Name = item.Name,
            FolderId = item.FolderId,
            OwnerId = item.OwnerId,
            SharingInfo = new() { Visibility = item.Visibility },
            Updated = item.Updated,
            Created = item.Created,
            Tags = item.Tags.Select(x => x.Value).ToArray(),
            Content = item.Content.Content,
            IsReadonly = isReadonly
        };
}
