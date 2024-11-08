using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollabDocumentEditor.Web.Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
   private readonly IAuthService _authService;
   private readonly ILogger<AuthController> _logger;
   
   public AuthController(IAuthService authService, ILogger<AuthController> logger)
   {
      _authService = authService ?? throw new ArgumentNullException(nameof(authService));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }


   [HttpPost("register")]
   public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
   {
      var result = await _authService.RegisterAsync(registerDto);

      if (!result.Succeeded)
      {
         _logger.LogError("Error occurred while registering user: {Errors}", result.Errors);
         return BadRequest(new { result.Errors});
      }
      
      _logger.LogInformation("User registered: {UserId}", result.UserId);
      return Ok(result);
   }

   [HttpPost("login")]
   public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
   {
      var result = await _authService.LoginAsync(loginDto); 
      
      if (!result.Succeeded)
      {
         _logger.LogError("Error occurred while logging in user: {Errors}", result.Errors);
         return BadRequest(new { result.Errors});
      }
      
      return Ok(result);
   }

   [Authorize(Policy = "DocumentRole")]
   [HttpPost("logout")]
   public async Task<IActionResult> Logout(Guid userId)
   {
      if (userId == Guid.Empty)
      {
         _logger.LogError("User id cannot be empty");
         return BadRequest("User id cannot be empty");
      }
      
      var result = await _authService.LogoutAsync(userId);
      
      if (!result)
      {
         _logger.LogError("Error occurred while logging out user: {UserId}", userId);
         return BadRequest("Error occurred while logging out user");
      }
         
      _logger.LogInformation("User logged out: {UserId}", userId);
      return Ok("User logged out");
   }

   [HttpGet("confirm-email")]
   public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
   {
      if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
      {
         _logger.LogError("Email or token cannot be empty");
         return BadRequest("Email or token cannot be empty");
      } 
      
      var result = await _authService.VerifyEmailAsync(email, token);
      
      if (!result.Succeeded)
      {
         _logger.LogError("Error occurred while verifying email: {Errors}", result.Errors);
         return BadRequest(new { result.Errors});
      }
      
      _logger.LogInformation("Email verified: {Email}", email);
      return Ok(result);
   }

   [HttpPost("forgot-password")]
   public async Task<IActionResult> ForgotPassword([FromBody] string email)
   {
      if (string.IsNullOrEmpty(email))
      {
         _logger.LogError("Email cannot be empty");
         return BadRequest("Email cannot be empty");
      }
      
      var result = await _authService.ForgotPasswordAsync(email);
      
      if (!result.Succeeded)
      {
         _logger.LogError("Error occurred while sending password reset email: {Errors}", result.Errors);
         return BadRequest(new { result.Errors});
      }
      
      _logger.LogInformation("Password reset email sent: {Email}", email);
      return Ok(result);
   }


   [HttpPost("reset-password")]
   public async Task<IActionResult> ResetPassword([FromQuery] string email,
      [FromQuery] string token,
      [FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
   {
     if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
     {
        _logger.LogError("Email or token cannot be empty");
        return BadRequest("Email or token cannot be empty");
     }  
     
     var resetPasswordDto = new ResetPasswordDto
     {
        Email = email,
        Token = token,
        NewPassword = resetPasswordRequestDto.NewPassword,
        ConfirmPassword = resetPasswordRequestDto.ConfirmPassword
     };
       
     var result = await _authService.ResetPasswordAsync(resetPasswordDto);
         
     if (!result.Succeeded)
     {
        _logger.LogError("Error occurred while resetting password: {Errors}", result.Errors);
        return BadRequest(new { result.Errors});
     }
         
     _logger.LogInformation("Password reset successfully: {Email}", email);
     return Ok(result);
   }
}