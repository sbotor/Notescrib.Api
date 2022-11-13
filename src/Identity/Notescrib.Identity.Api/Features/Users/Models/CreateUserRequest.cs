using Notescrib.Identity.Features.Users.Commands;

namespace Notescrib.Identity.Api.Features.Users.Models;

public class CreateUserRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PasswordConfirmation { get; set; } = null!;

    public CreateUser.Command ToCommand()
        => new(Email, Password, PasswordConfirmation);
}
