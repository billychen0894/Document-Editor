namespace CollabDocumentEditor.Core.Entities.Email;

public class EmailTemplate
{
    public string Subject { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public string PlainTextContent { get; set; } = string.Empty;
}