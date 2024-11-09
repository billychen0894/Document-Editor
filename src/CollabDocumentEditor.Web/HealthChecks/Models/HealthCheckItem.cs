namespace CollabDocumentEditor.Web.HealthChecks.Models;

public class HealthCheckItem
{
    public string Component { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
}