using CollabDocumentEditor.Core.Enum;

namespace CollabDocumentEditor.Core.Entities;

public class Document
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid UserId { get; set; } // Foreign key: document owner
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public virtual ApplicationUser User { get; set; } // Navigation property
    public virtual ICollection<DocumentUserPermission> DocumentUserPermissions { get; set; }
}