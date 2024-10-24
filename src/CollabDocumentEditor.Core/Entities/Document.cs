namespace CollabDocumentEditor.Core.Entities;

public class Document
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string UserId { get; set; } // Foreign key
    public virtual ApplicationUser User { get; set; } // Navigation property
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}