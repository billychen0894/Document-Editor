using SendGrid;
using SendGrid.Helpers.Mail;

namespace CollabDocumentEditor.Core.Interfaces.Services;

public interface ISendGridClientWrapper
{
    Task<Response> SendEmailAsync(SendGridMessage message, CancellationToken cancellationToken = default);
}