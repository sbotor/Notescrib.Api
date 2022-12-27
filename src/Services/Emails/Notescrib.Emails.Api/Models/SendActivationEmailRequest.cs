using Notescrib.Emails.Commands;

namespace Notescrib.Emails.Api.Models;

public class SendActivationEmailRequest
{
    public string TargetAddress { get; set; } = null!;
    public string CallbackUri { get; set; } = null!;

    public SendActivationEmail.Command ToCommand()
        => new(CallbackUri, TargetAddress);
}
