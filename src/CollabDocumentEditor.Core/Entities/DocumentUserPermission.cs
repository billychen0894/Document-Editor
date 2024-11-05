using CollabDocumentEditor.Core.Enum;

namespace CollabDocumentEditor.Core.Entities;

public class DocumentUserPermission
{
    public Guid Id { get; set; }
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public DocumentRole Role { get; set; }
    public DateTime GrantedAt { get; set; }
    public Guid GrantedBy { get; set; }
    public DateTime? RevokedAt { get; set; }
    public Guid? RevokedBy { get; set; }
    
    // Navigation properties
    public virtual Document Document { get; set; }
    public virtual ApplicationUser User { get; set; }
}