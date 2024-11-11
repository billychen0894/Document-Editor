using System.Net;
using CollabDocumentEditor.UnitTests.Base;
using Microsoft.Extensions.Logging;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CollabDocumentEditor.UnitTests.Services.EmailService;

public class SendEmailTests : EmailServiceTestsBase
{
   [Fact]
   public async Task SendEmailAsync_SuccessfulSend_LogsSuccess()
   {
      // Arrange
      var response = new Response(HttpStatusCode.OK, new StringContent(""), null);

      SendGridClientMock
         .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
         .ReturnsAsync(response);
      
      // Act
      await EmailService.SendEmailAsync("test@recipient.com", "Test Subject", "Test Message");

      // Assert
      SendGridClientMock.Verify(
         x => x.SendEmailAsync(
            It.Is<SendGridMessage>(msg =>
               msg.From.Email == EmailSettings.FromEmail &&
               msg.Subject == "Test Subject" &&
               msg.PlainTextContent == "Test Message"
            ),
            It.IsAny<CancellationToken>()
         ),
         Times.Once
      );

      VerifyLogger(LogLevel.Information, "Email sent successfully to test@recipient.com", Times.Once());
   }
   
   private void VerifyLogger(LogLevel level, string message, Times times)
   {
      LoggerMock.Verify(
         x => x.Log(
            level,
            It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(message)),
            It.IsAny<Exception>(),
            It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)!
         ),
         times
      );
   }
}