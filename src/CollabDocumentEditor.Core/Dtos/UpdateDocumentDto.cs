namespace CollabDocumentEditor.Core.Dtos;

public class UpdateDocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    // UpdatedAt will be set by service
}