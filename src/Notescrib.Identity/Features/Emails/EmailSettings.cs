namespace Notescrib.Identity.Features.Emails;

public class EmailSettings
{
    public string From { get; set; } = null!;
    public string Host { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool SkipEmails { get; set; }
    public CallbackUriTemplateSettings CallbackUriTemplates { get; set; } = new();
    
    public class CallbackUriTemplateSettings
    {
        public string ActivateAccount { get; set; } = null!;
        public string ResetPassword { get; set; } = null!;
    }
}
