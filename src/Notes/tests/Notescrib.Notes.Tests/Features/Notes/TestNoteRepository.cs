using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Tests.Features.Notes;

public class TestNoteRepository : TestRepositoryBase<Note, NotesSorting>, INoteRepository
{
    public Task<PagedList<Note>> GetNotesAsync(
        string? workspaceId,
        string? folderId,
        IPermissionGuard permissionGuard,
        PagingSortingInfo<NotesSorting> info,
        CancellationToken cancellationToken = default)
    {
        bool Filter(Note x)
            => (workspaceId == null || x.WorkspaceId == workspaceId)
               && (folderId == null || x.FolderId == folderId)
               && permissionGuard.ExpressionCanView<Note>()
                .Compile().Invoke(x);

        return GetPaged(Filter, info);
    }

    public Task AddNote(Note note, CancellationToken cancellationToken = default)
        => Add(note, x => x.Id = Guid.NewGuid().ToString());

    public Task<bool> ExistsAsync(
        string workspaceId,
        string folderId,
        string name,
        CancellationToken cancellationToken = default)
        => Exists(x => x.WorkspaceId == workspaceId && x.FolderId == folderId && x.Name == name);

    public Task<Note?> GetNoteByIdAsync(string id, CancellationToken cancellationToken = default)
        => GetSingleOrDefault(x => x.Id == id);

    public Task UpdateNoteAsync(Note note, CancellationToken cancellationToken = default)
        => Update(note, x => x.Id == note.Id);

    public Task DeleteNotesFromWorkspaceAsync(string workspaceId, IEnumerable<string>? folderIds = null, CancellationToken cancellationToken = default)
        => Delete(x => x.WorkspaceId == workspaceId
            && (folderIds == null || folderIds.Contains(x.FolderId)));
}
