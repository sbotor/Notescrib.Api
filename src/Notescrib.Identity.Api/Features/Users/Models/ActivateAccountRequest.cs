using Notescrib.Identity.Features.Users.Commands;

namespace Notescrib.Identity.Api.Features.Users.Models;

public class ActivateAccountRequest
{
    public string Token { get; set; } = null!;

    public ActivateAccount.Command ToCommand(string id)
        => new(id, Token);
}
