using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notescrib.Identity.Clients.Config;
using Notescrib.Identity.Clients.Models;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Notescrib.Identity.Clients;

public interface IEmailsApiClient
{
    Task<bool> SendActivationEmailAsync(string to, string userId, string token);
    Task<bool> SendResetPasswordEmailAsync(string to, string userId, string token);
}

public class EmailsApiClient : IEmailsApiClient
{
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, x => TimeSpan.FromMilliseconds(Math.Pow(x * 10, 2)));

    private readonly ILogger<EmailsApiClient> _logger;
    private readonly HttpClient _client;
    private readonly EmailsApiSettings _settings;

    public EmailsApiClient(IHttpClientFactory factory, IOptions<EmailsApiSettings> options,
        ILogger<EmailsApiClient> logger)
    {
        _logger = logger;
        _client = factory.CreateClient(nameof(EmailsApiClient));
        _settings = options.Value;
    }

    public async Task<bool> SendActivationEmailAsync(string to, string userId, string token)
    {
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var encodedUserId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId));

        var uri = string.Format(_settings.CallbackUriTemplates.ActivateAccount, encodedUserId, encodedToken);
        var request = new SendCallbackEmailRequest(to, uri);

        return await SendEmail(request, _settings.Paths.ActivationEmail);
    }

    public async Task<bool> SendResetPasswordEmailAsync(string to, string userId, string token)
    {
        var encodedToken = Encode(token);
        var encodedUserId = Encode(userId);

        var uri = string.Format(_settings.CallbackUriTemplates.ResetPassword, encodedUserId, encodedToken);
        var data = new SendCallbackEmailRequest(to, uri);

        return await SendEmail(data, _settings.Paths.ResetPasswordEmail);
    }

    private async Task<bool> SendEmail<T>(T request, string path)
    {
        using var response = await RetryPolicy.ExecuteAsync(() => Execute(request, path));

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        _logger.LogWarning("Emails API error: {code}\n{error}", response.StatusCode,
            await response.Content.ReadAsStringAsync());

        return false;
    }

    private async Task<HttpResponseMessage> Execute<T>(T request, string path)
    {
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, path);

        httpRequest.Content = JsonContent.Create(request);

        return await _client.SendAsync(httpRequest);
    }

    private static string Encode(string text)
        => WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(text));
}
