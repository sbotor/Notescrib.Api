using Notescrib.Identity.Features.Users.Commands;

namespace Notescrib.Identity.Api.Features.Users.Models;

public class InitiatePasswordResetRequest
{
    public string? Email { get; set; }

    public InitiatePasswordReset.Command ToCommand()
        => new(Email);
}
