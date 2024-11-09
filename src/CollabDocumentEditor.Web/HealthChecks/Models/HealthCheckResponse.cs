namespace CollabDocumentEditor.Web.HealthChecks.Models;

public class HealthCheckResponse
{
    public string Status { get; set; } = string.Empty;
    public IEnumerable<HealthCheckItem> Checks { get; set; } = Enumerable.Empty<HealthCheckItem>();
    public string TotalDuration { get; set; } = string.Empty;
}