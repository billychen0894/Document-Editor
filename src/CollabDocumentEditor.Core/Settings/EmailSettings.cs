namespace CollabDocumentEditor.Core.Settings;

public class EmailSettings
{
    public string SendGridApiKey { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public int RetryCount { get; set; } = 3;
    public int RetryDelayMilliseconds { get; set; } = 1000;
    public bool SandboxMode { get; set; }
    
    public string Host { get; set; } = string.Empty;

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(SendGridApiKey)
               && !string.IsNullOrWhiteSpace(FromEmail)
               && !string.IsNullOrWhiteSpace(FromName)
               && !string.IsNullOrWhiteSpace(Host);
    }
}