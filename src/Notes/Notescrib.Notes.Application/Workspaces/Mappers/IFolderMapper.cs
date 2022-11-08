using Notescrib.Notes.Application.Workspaces.Commands;
using Notescrib.Notes.Application.Workspaces.Models;
using Notescrib.Notes.Core.Entities;

namespace Notescrib.Notes.Application.Workspaces.Mappers;

public interface IFolderMapper
{
    FolderOverview MapToOverview(Folder item);
    Folder MapToEntity(CreateFolder.Command item, string ownerId, SharingInfo sharingInfo);
    Folder UpdateEntity(UpdateFolder.Command item, Folder original);
}
