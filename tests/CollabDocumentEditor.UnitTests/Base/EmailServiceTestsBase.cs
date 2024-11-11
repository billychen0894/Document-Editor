using CollabDocumentEditor.Core.Interfaces.Services;
using CollabDocumentEditor.Core.Settings;
using CollabDocumentEditor.Infrastructure.Configuration;
using CollabDocumentEditor.Infrastructure.Services;
using CollabDocumentEditor.UnitTests.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid;

namespace CollabDocumentEditor.UnitTests.Base;

public class EmailServiceTestsBase
{
   protected readonly EmailSettings EmailSettings;
   protected readonly Mock<ILogger<EmailService>> LoggerMock;
   protected readonly Mock<ISendGridClientWrapper> SendGridClientMock;
   protected readonly EmailService EmailService;

   protected EmailServiceTestsBase()
   {
       EmailSettings = new EmailSettings
       {
           SendGridApiKey = TestConstants.EmailConstants.ValidSendGridApiKey,
           FromEmail = TestConstants.EmailConstants.ValidFromEmail,
           FromName = TestConstants.EmailConstants.ValidFromName,
           RetryCount = TestConstants.EmailConstants.ValidRetryCount,
           RetryDelayMilliseconds = TestConstants.EmailConstants.ValidRetryDelayMilliseconds,
           SandboxMode = TestConstants.EmailConstants.ValidSandboxMode,
           Host = TestConstants.EmailConstants.ValidHost
       };

       LoggerMock = new Mock<ILogger<EmailService>>();

       SendGridClientMock = new Mock<ISendGridClientWrapper>();

       EmailService = new EmailService(
           Options.Create(EmailSettings),
           LoggerMock.Object,
           SendGridClientMock.Object
           );
   }
}