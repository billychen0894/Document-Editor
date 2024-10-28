using System.Net;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using SendGrid;

namespace CollabDocumentEditor.Infrastructure.HealthChecks;

public class SendGridHealthCheck: IHealthCheck
{
    private readonly SendGridClient _client;
    
    public SendGridHealthCheck(SendGridClient client)
    {
        _client = client;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _client.RequestAsync(
                method: BaseClient.Method.GET,
                urlPath: "scopes",
                cancellationToken: cancellationToken
            );

            return response.StatusCode == HttpStatusCode.OK
                ? HealthCheckResult.Healthy("SendGrid API is accessible")
                : HealthCheckResult.Unhealthy($"SendGrid API returned {response.StatusCode}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Failed to connect to SendGrid API", ex);
        }
    }
}