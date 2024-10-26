namespace CollabDocumentEditor.Core.Dtos;

public class CreateDocumentDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    // UserId will be set from the authenticated user
    // CreatedAt and UpdatedAt will be set by service
}