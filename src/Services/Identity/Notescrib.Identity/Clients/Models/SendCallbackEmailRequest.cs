namespace Notescrib.Identity.Clients.Models;

public record SendCallbackEmailRequest(string TargetAddress, string CallbackUri);
