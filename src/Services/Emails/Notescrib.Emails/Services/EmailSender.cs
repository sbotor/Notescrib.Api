using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Notescrib.Emails.Services;

public interface IEmailSender : IDisposable
{
    Task SendAsync(string to, string subject, string body);
}

public class EmailSender : IEmailSender
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

    public async Task SendAsync(string to, string subject, string body)
    {
        if (_settings.SkipEmails)
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
