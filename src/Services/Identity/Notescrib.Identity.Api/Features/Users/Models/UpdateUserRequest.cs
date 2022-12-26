using Notescrib.Identity.Features.Users.Commands;

namespace Notescrib.Identity.Api.Features.Users.Models;

public class UpdateUserRequest
{
    public string Email { get; set; } = null!;

    public UpdateUser.Command ToCommand()
        => new(Email);
}
