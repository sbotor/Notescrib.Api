using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notescrib.Identity.Clients.Config;

namespace Notescrib.Identity.Clients;

public interface INotesApiClient
{
    Task<bool> CreateWorkspace(string jwt);
}

public class NotesApiClient : INotesApiClient
{
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

    public async Task<bool> CreateWorkspace(string jwt)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _settings.WorkspacesPath);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        var response = await _client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        _logger.LogWarning("Notes API error: {code}\n{error}", response.StatusCode,
            await response.Content.ReadAsStringAsync());

        return false;
    }
}
