using Notescrib.Identity.Features.Users.Commands;

namespace Notescrib.Identity.Api.Features.Users.Models;

public class ConfirmEmailRequest
{
    public string Token { get; set; } = null!;

    public ConfirmEmail.Command ToCommand(string id)
        => new(id, Token);
}
