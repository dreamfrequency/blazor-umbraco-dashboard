using BlazorUmbracoDashboard.Domain.Entities;

namespace BlazorUmbracoDashboard.Application.Interfaces;

public interface IContentNodeRepository : IRepository<ContentNode>
{
    Task<IReadOnlyList<ContentNode>> GetRootNodesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ContentNode>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
}
