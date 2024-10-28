using CollabDocumentEditor.Core.Settings;
using SendGrid;

namespace CollabDocumentEditor.Infrastructure.Configuration;

public class SendGridClientOptionsSetup
{
    public static SendGridClientOptions CreateSendGridClientOptions(EmailSettings emailSettings)
    {
        if (emailSettings == null)
        {
            throw new ArgumentNullException(nameof(emailSettings));
        }

        var sendGridClientOptions = new SendGridClientOptions
        {
            ApiKey = emailSettings.SendGridApiKey,
            HttpErrorAsException = true
        };
        
        return sendGridClientOptions;
    }
}