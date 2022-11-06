using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Workspaces.Mappers;

public interface IFolderMapper
{
    FolderOverview MapToOverview(Folder item);
    Folder MapToEntity(CreateFolder.Command item, string ownerId, SharingInfo sharingInfo);
    Folder UpdateEntity(UpdateFolder.Command item, Folder original);
}
