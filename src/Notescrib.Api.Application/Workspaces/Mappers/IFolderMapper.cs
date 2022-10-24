using AutoMapper;
using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal interface IFolderMapper : IMapperBase
{
    FolderDetails MapToDetails(Folder folder, IEnumerable<NoteOverview> notes);
    IReadOnlyCollection<FolderOverview> MapToTree(IEnumerable<Folder> folders);
}
