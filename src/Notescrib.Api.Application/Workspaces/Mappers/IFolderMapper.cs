using AutoMapper;
using Notescrib.Api.Application.Notes.Models;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

internal interface IFolderMapper : IMapperBase
{
    FolderDetails MapToDetails(Folder folder, IEnumerable<NoteOverview> notes);
    Folder Update(Folder original, UpdateFolder.Command command);
}
