namespace Notescrib.Identity.Clients.Config;

public class EmailsApiSettings
{
    public Uri BaseUrl { get; set; } = null!;
    public string EmailsPath { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string ConfirmationUriTemplate { get; set; } = null!;
}
