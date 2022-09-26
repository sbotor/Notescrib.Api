using Notescrib.Api.Application.Contracts.Workspace;

namespace Notescrib.Api.Application.Services;
internal interface INoteService
{
    Task<IReadOnlyCollection<NoteDetails>> GetNoteDetailsAsync(string path);
}