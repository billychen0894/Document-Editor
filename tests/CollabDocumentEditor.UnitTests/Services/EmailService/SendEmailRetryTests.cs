using System.Net;
using CollabDocumentEditor.Core.Exceptions;
using CollabDocumentEditor.UnitTests.Base;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CollabDocumentEditor.UnitTests.Services.EmailService;

public class SendEmailRetryTests : EmailServiceTestsBase
{
   
    [Fact]
    public async Task SendEmailAsync_FailureWithRetry_EventuallySucceeds()
    {
        // Arrange
        var failedResponse = new Response(HttpStatusCode.ServiceUnavailable, new StringContent(""), null);
        var successResponse = new Response(HttpStatusCode.OK, new StringContent(""), null);
        
        SendGridClientMock
            .SetupSequence(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResponse)
            .ReturnsAsync(successResponse);

        // Act
        await EmailService.SendEmailAsync("test@recipient.com", "Test Subject", "Test Message");

        // Assert
        SendGridClientMock.Verify(
            x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
    } 
    
    [Fact]
    public async Task SendEmailAsync_ExceedsRetryCount_ThrowsEmailServiceException()
    {
        // Arrange
        var failedResponse = new Response(HttpStatusCode.ServiceUnavailable, new StringContent(""), null);
        
        SendGridClientMock
            .Setup(x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(failedResponse);

        // Act & Assert
        await Assert.ThrowsAsync<EmailServiceException>(() =>
            EmailService.SendEmailAsync("test@recipient.com", "Test Subject", "Test Message")
        );
        
        SendGridClientMock.Verify(
            x => x.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()),
            Times.Exactly(EmailSettings.RetryCount)
        );
    }
}