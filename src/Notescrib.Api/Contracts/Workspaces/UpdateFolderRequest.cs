using Notescrib.Api.Application.Workspaces.Commands;

namespace Notescrib.Api.Contracts.Workspaces;

public class UpdateFolderRequest : ModifyFolderRequestBase
{
    public UpdateFolder.Command ToCommand(string folderId)
        => new()
        {
            Id = folderId,
            Name = Name,
            ParentId = ParentId,
            SharingInfo = SharingInfo
        };
}
