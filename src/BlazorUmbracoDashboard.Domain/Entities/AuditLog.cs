using BlazorUmbracoDashboard.Domain.Common;

namespace BlazorUmbracoDashboard.Domain.Entities;

public class AuditLog : BaseEntity
{
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public Guid? EntityId { get; set; }
    public string PerformedBy { get; set; } = string.Empty;
    public string? Details { get; set; }
}
