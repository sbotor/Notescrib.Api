using Notescrib.Identity.Features.Users.Commands;

namespace Notescrib.Identity.Api.Features.Users.Models;

public class ResetUserPasswordRequest
{
    public string Token { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PasswordConfirmation { get; set; } = null!;

    public ResetUserPassword.Command ToCommand(string id)
        => new(id, Token, Password, PasswordConfirmation);
}
