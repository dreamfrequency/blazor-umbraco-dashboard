using AutoMapper;
using BlazorUmbracoDashboard.Application.DTOs;
using BlazorUmbracoDashboard.Application.Interfaces;
using BlazorUmbracoDashboard.Domain.Entities;

namespace BlazorUmbracoDashboard.Application.Services;

public class ContentService
{
    private readonly IContentNodeRepository _contentRepo;
    private readonly IRepository<AuditLog> _auditRepo;
    private readonly IMapper _mapper;

    public ContentService(
        IContentNodeRepository contentRepo,
        IRepository<AuditLog> auditRepo,
        IMapper mapper)
    {
        _contentRepo = contentRepo;
        _auditRepo = auditRepo;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ContentNodeDto>> GetAllContentAsync(CancellationToken ct = default)
    {
        var nodes = await _contentRepo.GetAllAsync(ct);
        return _mapper.Map<IReadOnlyList<ContentNodeDto>>(nodes);
    }

    public async Task<IReadOnlyList<ContentNodeDto>> GetRootContentAsync(CancellationToken ct = default)
    {
        var nodes = await _contentRepo.GetRootNodesAsync(ct);
        return _mapper.Map<IReadOnlyList<ContentNodeDto>>(nodes);
    }

    public async Task<ContentNodeDto?> GetContentByIdAsync(Guid id, CancellationToken ct = default)
    {
        var node = await _contentRepo.GetByIdAsync(id, ct);
        return node is null ? null : _mapper.Map<ContentNodeDto>(node);
    }

    public async Task<ContentNodeDto> CreateContentAsync(ContentNodeDto dto, string performedBy, CancellationToken ct = default)
    {
        var entity = _mapper.Map<ContentNode>(dto);
        var created = await _contentRepo.AddAsync(entity, ct);

        await _auditRepo.AddAsync(new AuditLog
        {
            Action = "Created",
            EntityType = nameof(ContentNode),
            EntityId = created.Id,
            PerformedBy = performedBy,
            Details = $"Created content node '{created.Name}'"
        }, ct);

        return _mapper.Map<ContentNodeDto>(created);
    }

    public async Task DeleteContentAsync(Guid id, string performedBy, CancellationToken ct = default)
    {
        await _contentRepo.DeleteAsync(id, ct);

        await _auditRepo.AddAsync(new AuditLog
        {
            Action = "Deleted",
            EntityType = nameof(ContentNode),
            EntityId = id,
            PerformedBy = performedBy,
            Details = $"Deleted content node {id}"
        }, ct);
    }
}
