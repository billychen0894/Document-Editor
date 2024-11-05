using CollabDocumentEditor.Core.Enum;

namespace CollabDocumentEditor.Core.Dtos;

public class ShareDocumentDto
{
    public Guid DocumentId { get; set; }
    public Guid UserId { get; set; }
    public DocumentRole Role { get; set; }
}