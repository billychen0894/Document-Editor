using System.Text.Encodings.Web;

namespace CollabDocumentEditor.Core.Entities.Email;

public class EmailTemplates
{
    public static EmailTemplate ConfirmEmail(string callbackUrl)
    {
        return new EmailTemplate
        {
            Subject = "Confirm your email",
            HtmlContent = $"""
                                           <html>
                                               <body>
                                                   <h2>Welcome!</h2>
                                                   <p>Please confirm your email by clicking the link below:</p>
                                                   <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Confirm Email</a></p>
                                                   <p>If you didn't request this, please ignore this email.</p>
                                               </body>
                                           </html>
                           """,
            PlainTextContent = $"Welcome! Please confirm your email by clicking this link: {callbackUrl}"
        };
    }

    public static EmailTemplate ResetPassword(string callbackUrl)
    {
        return new EmailTemplate
        {
            Subject = "Reset your password",
            HtmlContent = $"""
                                           <html>
                                               <body>
                                                   <h2>Reset Password</h2>
                                                   <p>To reset your password, click the link below:</p>
                                                   <p><a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Reset Password</a></p>
                                                   <p>If you didn't request this, please ignore this email.</p>
                                               </body>
                                           </html>
                           """,
            PlainTextContent = $"To reset your password, click this link: {callbackUrl}"
        };
    }
}