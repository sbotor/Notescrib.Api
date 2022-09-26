using Notescrib.Api.Application.Contracts.Workspace;

namespace Notescrib.Api.Application.Services;

internal class NoteService : INoteService
{
    public async Task<IReadOnlyCollection<NoteDetails>> GetNoteDetailsAsync(string path)
    {
        return new List<NoteDetails>();
    }
}
