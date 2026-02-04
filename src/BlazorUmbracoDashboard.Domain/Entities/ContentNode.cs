using BlazorUmbracoDashboard.Domain.Common;

namespace BlazorUmbracoDashboard.Domain.Entities;

public class ContentNode : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft";
    public string? UmbracoId { get; set; }
    public Guid? ParentId { get; set; }
    public ContentNode? Parent { get; set; }
    public ICollection<ContentNode> Children { get; set; } = new List<ContentNode>();
}
