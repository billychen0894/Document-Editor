using CollabDocumentEditor.Core.Dtos.AuthDtos;

namespace CollabDocumentEditor.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterDto registerDto);
    Task<AuthResult> LoginAsync(LoginDto loginDto);
    Task<bool> LogoutAsync(string userId);
    Task<AuthResult> VerifyEmailAsync(string email, string token);
    Task<AuthResult> ForgotPasswordAsync(string email);
    Task<AuthResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}