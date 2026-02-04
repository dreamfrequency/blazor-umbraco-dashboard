using BlazorUmbracoDashboard.Application.Interfaces;
using BlazorUmbracoDashboard.Domain.Entities;
using BlazorUmbracoDashboard.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorUmbracoDashboard.Infrastructure.Repositories;

public class ContentNodeRepository : Repository<ContentNode>, IContentNodeRepository
{
    public ContentNodeRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ContentNode>> GetRootNodesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.ParentId == null)
            .Include(n => n.Children)
            .OrderBy(n => n.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ContentNode>> GetChildrenAsync(Guid parentId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.ParentId == parentId)
            .Include(n => n.Children)
            .OrderBy(n => n.Name)
            .ToListAsync(cancellationToken);
    }
}
