using CollabDocumentEditor.Web.HealthChecks.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CollabDocumentEditor.Web.HealthChecks;

public class HealthCheckResponseWriter
{
    public static async Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        
        var response = new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(e => new HealthCheckItem
            {
                Component = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description,
                Duration = e.Value.Duration.ToString(),
                Tags = e.Value.Tags
            }),
            TotalDuration = report.TotalDuration.ToString()
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}