using BlazorUmbracoDashboard.Application.DTOs;

namespace BlazorUmbracoDashboard.Application.Interfaces;

public interface IUmbracoApiClient
{
    Task<IReadOnlyList<ContentNodeDto>> GetContentNodesAsync(CancellationToken cancellationToken = default);
    Task<ContentNodeDto?> GetContentNodeAsync(string umbracoId, CancellationToken cancellationToken = default);
    Task<bool> PublishContentAsync(string umbracoId, CancellationToken cancellationToken = default);
    Task<bool> UnpublishContentAsync(string umbracoId, CancellationToken cancellationToken = default);
}
