using CollabDocumentEditor.Core.Interfaces.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CollabDocumentEditor.Infrastructure.Services;

public class SendGridClientWrapper : ISendGridClientWrapper
{
    private readonly SendGridClient _client;

    public SendGridClientWrapper(SendGridClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public Task<Response> SendEmailAsync(SendGridMessage message, CancellationToken cancellationToken = default)
    {
        return _client.SendEmailAsync(message, cancellationToken);
    } 
}