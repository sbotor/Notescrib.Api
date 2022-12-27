namespace Notescrib.Identity.Clients.Config;

public class EmailsApiSettings
{
    public Uri BaseUrl { get; set; } = null!;
    public PathSettings Paths { get; set; } = new();
    public string ApiKey { get; set; } = null!;
    public CallbackUriTemplateSettings CallbackUriTemplates { get; set; } = new();

    public class PathSettings
    {
        public string ActivationEmail { get; set; } = null!;
        public string ResetPasswordEmail { get; set; } = null!;
    }

    public class CallbackUriTemplateSettings
    {
        public string ActivateAccount { get; set; } = null!;
        public string ResetPassword { get; set; } = null!;
    }
}
