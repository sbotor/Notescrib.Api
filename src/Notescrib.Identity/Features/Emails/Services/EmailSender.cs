using System.Text;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Notescrib.Emails.Services;

public interface IEmailSender
{
    Task SendActivationEmailAsync(string to, string userId, string token);
    Task SendPasswordResetEmailAsync(string to, string userId, string token);
}

public sealed class EmailSender : IEmailSender, IDisposable
{
    private readonly EmailSettings _settings;
    private readonly Lazy<SmtpClient> _client;
    private readonly InternetAddress _fromAddress;

    public EmailSender(IOptions<EmailSettings> options)
    {
        _settings = options.Value;

        _fromAddress = new MailboxAddress("Notescrib", _settings.From);

        _client = new(() => new SmtpClient());
    }

    public Task SendActivationEmailAsync(string to, string userId, string token)
    {
        var uri = EncodeAndFormat(_settings.CallbackUriTemplates.ActivateAccount, userId, token);

        return SendAsync(to, "Activate account",
            $"Confirm your email here: {uri}");
    }

    public Task SendPasswordResetEmailAsync(string to, string userId, string token)
    {
        var uri = EncodeAndFormat(_settings.CallbackUriTemplates.ResetPassword, userId, token);
        
        return SendAsync(to, "Reset password",
            $"Reset your password here: {uri}");
    }

    private async Task SendAsync(string to, string subject, string body)
    {
        if (_settings.SkipEmails
            || string.IsNullOrEmpty(_settings.From))
        {
            return;
        }
        
        var message = new MimeMessage();
        message.From.Add(_fromAddress);
        message.To.Add(new MailboxAddress(to, to));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        var client = _client.Value;
        if (!client.IsConnected)
        {
            await client.ConnectAsync(_settings.Host, 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.From, _settings.Password);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
    
    private static string Base64UrlEncode(string value)
        => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(value));

    private static string EncodeAndFormat(string template, string userId, string token)
    {
        var encodedToken = Base64UrlEncode(token);
        var encodedUserId = Base64UrlEncode(userId);
        
        return string.Format(template, encodedUserId, encodedToken);
    }

    public void Dispose()
    {
        if (!_client.IsValueCreated)
        {
            return;
        }

        _client.Value.Disconnect(true);
        _client.Value.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
