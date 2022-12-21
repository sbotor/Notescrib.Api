using Notescrib.Emails.Commands;

namespace Notescrib.Emails.Api.Models;

public class SendEmailConfirmationRequest
{
    public string Email { get; set; } = null!;
    public string Uri { get; set; } = null!;

    public SendEmailConfirmation.Command ToCommand()
        => new(Uri, Email);
}
