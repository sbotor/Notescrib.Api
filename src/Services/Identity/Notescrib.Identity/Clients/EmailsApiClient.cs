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
    Task<bool> SendConfirmationEmailAsync(string to, string userId, string token);
}

public class EmailsApiClient : IEmailsApiClient
{
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, x => TimeSpan.FromMilliseconds(Math.Pow(x * 10, 2)));
    
    private readonly ILogger<EmailsApiClient> _logger;
    private readonly HttpClient _client;
    private readonly EmailsApiSettings _settings;
    
    public EmailsApiClient(IHttpClientFactory factory, IOptions<EmailsApiSettings> options, ILogger<EmailsApiClient> logger)
    {
        _logger = logger;
        _client = factory.CreateClient(nameof(EmailsApiClient));
        _settings = options.Value;
    }
    
    public async Task<bool> SendConfirmationEmailAsync(string to, string userId, string token)
    {
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var encodedUserId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(userId));
        var uri = string.Format(_settings.ConfirmationUriTemplate, encodedUserId, encodedToken);
        var data = new SendConfirmationEmailsRequest(to, uri);
        
        using var response = await RetryPolicy.ExecuteAsync(() => Execute(data));
        
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        _logger.LogWarning("Emails API error: {code}\n{error}", response.StatusCode,
            await response.Content.ReadAsStringAsync());

        return false;
    }

    private async Task<HttpResponseMessage> Execute(SendConfirmationEmailsRequest data)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _settings.EmailsPath);
        
        request.Content = JsonContent.Create(data);
        
        return await _client.SendAsync(request);
    }
}
