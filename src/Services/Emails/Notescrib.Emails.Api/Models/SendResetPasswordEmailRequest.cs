using Notescrib.Emails.Commands;

namespace Notescrib.Emails.Api.Models;

public class SendResetPasswordEmailRequest
{
    public string TargetAddress { get; set; } = null!;
    public string CallbackUri { get; set; } = null!;

    public SendResetPasswordEmail.Command ToCommand()
        => new(CallbackUri, TargetAddress);
}
