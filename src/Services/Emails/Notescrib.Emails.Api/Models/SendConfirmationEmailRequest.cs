using Notescrib.Emails.Commands;

namespace Notescrib.Emails.Api.Models;

public class SendConfirmationEmailRequest
{
    public string Email { get; set; } = null!;
    public string Uri { get; set; } = null!;

    public SendConfirmationEmail.Command ToCommand()
        => new(Uri, Email);
}
