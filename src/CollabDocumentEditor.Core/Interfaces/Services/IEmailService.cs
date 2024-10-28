namespace CollabDocumentEditor.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendEmailConfirmationAsync(string email, string callbackUrl);
    Task SendPasswordResetEmailAsync(string email, string callbackUrl);
}