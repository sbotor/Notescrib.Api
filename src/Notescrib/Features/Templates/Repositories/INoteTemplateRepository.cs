using Notescrib.Features.Templates.Utils;
using Notescrib.Models;
using Notescrib.Utils;

namespace Notescrib.Features.Templates.Repositories;

public interface INoteTemplateRepository
{
    Task<NoteTemplate?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task CreateAsync(NoteTemplate template, CancellationToken cancellationToken = default);
    Task UpdateAsync(NoteTemplate template, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    Task UpdateContentAsync(NoteTemplate template, CancellationToken cancellationToken = default);

    Task<PagedList<NoteTemplate>> SearchAsync(string? textFilter, PagingSortingInfo<NoteTemplatesSorting> pagingSortingInfo,
        CancellationToken cancellationToken);

    Task DeleteAllAsync(CancellationToken cancellationToken = default);
}
