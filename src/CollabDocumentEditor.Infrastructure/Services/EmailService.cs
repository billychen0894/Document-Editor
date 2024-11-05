using CollabDocumentEditor.Core.Entities.Email;
using CollabDocumentEditor.Core.Exceptions;
using CollabDocumentEditor.Core.Interfaces.Services;
using CollabDocumentEditor.Core.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CollabDocumentEditor.Infrastructure.Services;

public class EmailService : IEmailService
{
   private readonly EmailSettings _emailSettings;
   private readonly ILogger<EmailService> _logger;
   private readonly SendGridClient _sendGridClient;
   
   public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger, SendGridClient sendGridClient)
   {
      _sendGridClient = sendGridClient ?? throw new ArgumentNullException(nameof(sendGridClient));
      _emailSettings = emailSettings.Value ?? throw new ArgumentNullException(nameof(emailSettings));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }

   private async Task<Response> SendWithRetryAsync(SendGridMessage sendGridMessage)
   {
      var attempts = 0;
      while (true)
      {
         try
         {
            attempts++;
            var response = await _sendGridClient.SendEmailAsync(sendGridMessage);

            if (response.IsSuccessStatusCode)
               return response;
            
            var responseBody = await response.Body.ReadAsStringAsync();
            _logger.LogError("SendGrid API returned non-success status code {StatusCode}. Response: {Response}",
               response.StatusCode,
               responseBody);

            if (attempts >= _emailSettings.RetryCount)
               throw new EmailServiceException($"Failed to send email after {attempts} attempts. " +
                                               $"Last status code: {response.StatusCode}");

            await Task.Delay(_emailSettings.RetryDelayMilliseconds * attempts);
         }
         catch (Exception ex) when (ex is not EmailServiceException)
         {
            _logger.LogError(ex, "Error occurred while sending email (Attempt {Attempt}/{MaxAttempts})", 
               attempts, 
               _emailSettings.RetryCount);
            
            if (attempts >= _emailSettings.RetryCount)
               throw new EmailServiceException(
                  $"Failed to send email after {attempts} attempts", ex);

            await Task.Delay(_emailSettings.RetryDelayMilliseconds * attempts);
         }
      }
   }

   
   public async Task SendEmailAsync(string email, string subject, string message)
   {
      try
      {
         var msg = new SendGridMessage
         {
            From = new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
            Subject = subject,
            PlainTextContent = message,
            HtmlContent = message
         };

         msg.AddTo(new EmailAddress(email));
         msg.SetClickTracking(false, false);
         msg.SetOpenTracking(false);
         msg.SetGoogleAnalytics(false);
         msg.SetSubscriptionTracking(false);

         if (_emailSettings.SandboxMode)
         {
            msg.MailSettings = new MailSettings
            {
               SandboxMode = new SandboxMode { Enable = true }
            };
         }

         await SendWithRetryAsync(msg);
         _logger.LogInformation("Email sent successfully to {Email}", email);
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error sending email to {Email}", email);
         throw;
      }
   }

   public async Task SendEmailConfirmationAsync(string email, string callbackUrl)
   {
      try
      {
         var template = EmailTemplates.ConfirmEmail($"{_emailSettings.Host}{callbackUrl}");

         var msg = new SendGridMessage
         {
            From = new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
            Subject = template.Subject,
            PlainTextContent = template.PlainTextContent,
            HtmlContent = template.HtmlContent
         };
         
         msg.AddTo(new EmailAddress(email));
         msg.SetClickTracking(false, false);
         msg.SetOpenTracking(false);
         msg.SetGoogleAnalytics(false);
         msg.SetSubscriptionTracking(false);

         if (_emailSettings.SandboxMode)
         {
            msg.MailSettings = new MailSettings
            {
               SandboxMode = new SandboxMode { Enable = true }
            };
         }

         await SendWithRetryAsync(msg);
         _logger.LogInformation("Confirmation email sent successfully to {Email}", email); 
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Failed to send confirmation email to {Email}", email);
         throw;
      }
   }

   public async Task SendPasswordResetEmailAsync(string email, string callbackUrl)
   {
      try
      {
         var template = EmailTemplates.ResetPassword($"{_emailSettings.Host}{callbackUrl}");

         var msg = new SendGridMessage
         {
            From = new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
            Subject = template.Subject,
            PlainTextContent = template.PlainTextContent,
            HtmlContent = template.HtmlContent
         };

         msg.AddTo(new EmailAddress(email));
         msg.SetClickTracking(false, false);
         msg.SetOpenTracking(false);
         msg.SetGoogleAnalytics(false);
         msg.SetSubscriptionTracking(false);

         if (_emailSettings.SandboxMode)
         {
            msg.MailSettings = new MailSettings
            {
               SandboxMode = new SandboxMode { Enable = true }
            };
         }

         await SendWithRetryAsync(msg);
         _logger.LogInformation("Password reset email sent successfully to {Email}", email); 
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Failed to send password reset email to {Email}", email);
         throw;
      }
   }
}