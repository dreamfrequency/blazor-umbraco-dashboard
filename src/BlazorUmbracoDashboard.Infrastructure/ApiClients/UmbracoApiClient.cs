using System.Net.Http.Json;
using BlazorUmbracoDashboard.Application.DTOs;
using BlazorUmbracoDashboard.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace BlazorUmbracoDashboard.Infrastructure.ApiClients;

public class UmbracoApiClient : IUmbracoApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UmbracoApiClient> _logger;

    public UmbracoApiClient(HttpClient httpClient, ILogger<UmbracoApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ContentNodeDto>> GetContentNodesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ContentNodeDto>>(
                "umbraco/delivery/api/v2/content", cancellationToken);
            return response ?? new List<ContentNodeDto>();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch content nodes from Umbraco API");
            return new List<ContentNodeDto>();
        }
    }

    public async Task<ContentNodeDto?> GetContentNodeAsync(string umbracoId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ContentNodeDto>(
                $"umbraco/delivery/api/v2/content/item/{umbracoId}", cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to fetch content node {UmbracoId} from Umbraco API", umbracoId);
            return null;
        }
    }

    public async Task<bool> PublishContentAsync(string umbracoId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"umbraco/management/api/v1/content/{umbracoId}/publish", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to publish content {UmbracoId}", umbracoId);
            return false;
        }
    }

    public async Task<bool> UnpublishContentAsync(string umbracoId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                $"umbraco/management/api/v1/content/{umbracoId}/unpublish", null, cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to unpublish content {UmbracoId}", umbracoId);
            return false;
        }
    }
}
