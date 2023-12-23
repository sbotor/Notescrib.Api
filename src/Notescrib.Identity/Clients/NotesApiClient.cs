using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notescrib.Identity.Clients.Config;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace Notescrib.Identity.Clients;

public interface INotesApiClient
{
    Task<bool> DeleteWorkspaceAsync(string jwt);
}

public class NotesApiClient : INotesApiClient
{
    private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(3, x => TimeSpan.FromMilliseconds(Math.Pow(x * 10, 2)));
    
    private readonly HttpClient _client;
    private readonly ILogger<NotesApiClient> _logger;
    private readonly NotesApiSettings _settings;

    public NotesApiClient(IHttpClientFactory clientFactory, IOptions<NotesApiSettings> options,
        ILogger<NotesApiClient> logger)
    {
        _client = clientFactory.CreateClient(nameof(NotesApiClient));
        _logger = logger;
        _settings = options.Value;
    }

    public Task<bool> DeleteWorkspaceAsync(string jwt)
        => SendWithMethod(jwt, HttpMethod.Delete);

    private async Task<bool> SendWithMethod(string jwt, HttpMethod method)
    {
        using var response = await RetryPolicy.ExecuteAsync(() => Execute(jwt, method));

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        _logger.LogWarning("Notes API error: {code}\n{error}", response.StatusCode,
            await response.Content.ReadAsStringAsync());

        return false;
    }

    private async Task<HttpResponseMessage> Execute(string jwt, HttpMethod method)
    {
        using var request = new HttpRequestMessage(method, "api/workspaces");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        return await _client.SendAsync(request);
    }
}
