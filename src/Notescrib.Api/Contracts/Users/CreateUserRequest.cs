using Notescrib.Api.Application.Users.Commands;

namespace Notescrib.Api.Contracts.Users;

public class CreateUserRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PasswordConfirmation { get; set; } = string.Empty;

    public AddUser.Command ToCommand()
        => new(Email, Password, PasswordConfirmation);
}
