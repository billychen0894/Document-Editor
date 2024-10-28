using AutoMapper;
using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Interfaces.Services;
using CollabDocumentEditor.Core.Settings;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CollabDocumentEditor.Infrastructure.Services;

public class AuthService : IAuthService
{
   private readonly UserManager<ApplicationUser> _userManager; 
   private readonly SignInManager<ApplicationUser> _signInManager;
   private readonly IMapper _mapper;
   private readonly IValidator<LoginDto> _loginDtoValidator;
   private readonly IValidator<RegisterDto> _registerDtoValidator;
   private readonly ILogger<AuthService> _logger;
   private readonly ITokenService _tokenService;
   private readonly JwtSettings _jwtSettings;
   private readonly IEmailService _emailService;

   public AuthService(
       UserManager<ApplicationUser> userManager,
       SignInManager<ApplicationUser> signInManager,
       IMapper mapper,
       IValidator<LoginDto> loginDtoValidator,
       IValidator<RegisterDto> registerDtoValidator,
       ILogger<AuthService> logger,
       ITokenService tokenService,
       IOptions<JwtSettings> jwtSettings,
       IEmailService emailService)
   {
     _userManager = userManager;
     _signInManager = signInManager;
     _mapper = mapper;
     _loginDtoValidator = loginDtoValidator;
     _registerDtoValidator = registerDtoValidator;
     _logger = logger;
     _tokenService = tokenService;
     _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
     _emailService = emailService;
   }


   /// <summary>
   /// Register new user
   /// </summary>
   /// <param name="registerDto">Register DTO</param>
   /// <returns>An AuthResult object</returns>
   public async Task<AuthResult> RegisterAsync(RegisterDto registerDto)
   {
       try
       {
           // Input validation
           if (registerDto == null) throw new ArgumentNullException(nameof(registerDto));
           
           _logger.LogInformation("Starting registration for user: {Email}", registerDto.Email);
           
           // Validate input
           var validationResult = await _registerDtoValidator.ValidateAsync(registerDto);
           if (!validationResult.IsValid)
           {
               return new AuthResult()
               {
                   Succeeded = false,
                   Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
               };
           }
       
           // Map RegisterDto to ApplicationUser
           var newUser = _mapper.Map<RegisterDto, ApplicationUser>(registerDto);
           if (newUser == null)
           {
               return new AuthResult()
               {
                   Succeeded = false,
                   Errors = new List<string> {"User mapping failed. Please try again."}
               };
           }
       
           // Check if user already exists
           var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
           if (existingUser != null)
           {
               return new AuthResult()
               {
                   Succeeded = false,
                   Errors = new List<string> { "User with this email already exists." }
               };
           }
               
           // Create user
           var identityResult = await _userManager.CreateAsync(newUser, registerDto.Password);
           if (!identityResult.Succeeded)
           {
               _logger.LogError("Failed to create user: {Email}", registerDto.Email);
               await _userManager.DeleteAsync(newUser);
               
               return new AuthResult()
               {
                   Succeeded = false,
                   Errors = identityResult.Errors.Select(e => e.Description).ToList()
               };
           }
       
           // Create tokens
           var accessToken = _tokenService.GenerateAccessToken(newUser);
           var refreshToken = _tokenService.GenerateRefreshToken();
           var now = DateTime.UtcNow;
       
           // Update user with refreshToken
           newUser.RefreshToken = refreshToken; 
           newUser.RefreshTokenExpiryTime = now.AddHours(_jwtSettings.RefreshTokenExpirationDays);
           var updateResult = await _userManager.UpdateAsync(newUser);

           if (!updateResult.Succeeded)
           {
               _logger.LogError("Failed to update user with refreshToken. User ID: {UserId}", newUser.Id);
               
               // Clean up - delete the created user due to errors
               await _userManager.DeleteAsync(newUser);
               
               return new AuthResult()
               {
                   Succeeded = false,
                   Errors = new List<string> {"Failed to complete user registration. Please try again."} 
               };
           }
           
           var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
           var emailConfirmationCallbackUrl = $"/confirm-email?email={Uri.EscapeDataString(newUser.Email)}&token={Uri.EscapeDataString(emailConfirmationToken)}";
           
           await _emailService.SendEmailConfirmationAsync(newUser.Email, emailConfirmationCallbackUrl);
           
           _logger.LogInformation("User successfully registered: {Email}", registerDto.Email);
           
           return new AuthResult()
           {
               Succeeded = true,
               AccessToken = accessToken,
               RefreshToken = refreshToken,
               AccessTokenExpiration = now.AddHours(_jwtSettings.AccessTokenExpirationHours),
               RefreshTokenExpiration = now.AddDays(_jwtSettings.RefreshTokenExpirationDays),
               UserId = newUser.Id,
               Email = newUser.Email,
           };
       }
       catch (Exception ex)
       {
          _logger.LogError(ex, "Error during user registration for email: {Email}", registerDto?.Email);
          return new AuthResult()
          {
              Succeeded = false,
              Errors = new List<string> { "An unexpected error occurred during registration. Please try again." }
          };
       }
   }

   /// <summary>
   /// User login
   /// </summary>
   /// <param name="loginDto"> Login DTO input</param>
   /// <returns>An AuthResult object</returns>
   public async Task<AuthResult> LoginAsync(LoginDto loginDto)
   {
       try
       {
           var validationResult = await _loginDtoValidator.ValidateAsync(loginDto);
           if (!validationResult.IsValid)
           {
               return new AuthResult
               {
                   Succeeded = false,
                   Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
               };
           }
           
           var user = await _userManager.FindByEmailAsync(loginDto.Email);
           if (user == null)
           {
               return new AuthResult
               {
                   Succeeded = false,
                   Errors = new List<string> { "User with this email does not exist." }
               };
           }
           
           var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
           if (!signInResult.Succeeded)
           {
               return new AuthResult
               {
                   Succeeded = false,
                   Errors = new List<string> { "Invalid password." }
               };
           }
           
           var accessToken = _tokenService.GenerateAccessToken(user);
           var refreshToken = _tokenService.GenerateRefreshToken();
           
           user.RefreshToken = refreshToken;
           user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
           
           var updateResult = await _userManager.UpdateAsync(user);
           if (!updateResult.Succeeded)
           {
               return new AuthResult
               {
                   Succeeded = false,
                   Errors = new List<string> { "Failed to complete user login process. Please try again." }
               };
           }

           return new AuthResult
           {
               Succeeded = true,
               AccessToken = accessToken,
               RefreshToken = refreshToken,
               AccessTokenExpiration = DateTime.UtcNow.AddHours(_jwtSettings.AccessTokenExpirationHours),
               RefreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
               UserId = user.Id,
               Email = user.Email,
           };
       }
       catch (Exception ex)
       {
          _logger.LogError(ex, "Error during login for user: {Email}", loginDto.Email);
          return new AuthResult
          {
              Succeeded = false,
              Errors = new List<string> { "An unexpected error occurred during login." }
          };
       }
   }

   /// <summary>
   /// Logout user account
   /// </summary>
   /// <param name="userId"></param>
   /// <returns>true for logout success</returns>
   public async Task<bool> LogoutAsync(string userId)
   {
       try
       {
           await _tokenService.RevokeTokenAsync(userId);
           return true;
       }
       catch (Exception ex)
       {
          _logger.LogError(ex, "Error during logout for user: {UserId}", userId); 
          return false;
       }
   }

   /// <summary>
   /// Verify user email with its associated email confirmation token
   /// </summary>
   /// <param name="email"></param>
   /// <param name="token"></param>
   /// <returns>An authentication result</returns>
   public async Task<AuthResult> VerifyEmailAsync(string email, string token)
   {
       try
       {
           if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(email), "Email and/or token are required.");
           
           var user = await _userManager.FindByEmailAsync(email);
           if (user == null)
           {
               return new AuthResult
               {
                   Succeeded = false,
                   Errors = new List<string> { "User not found." }
               };
           }
           
           var result = await _userManager.ConfirmEmailAsync(user, token);
           if (!result.Succeeded)
           {
               return new AuthResult
               {
                   Succeeded = false,
                   Errors = result.Errors.Select(e => e.Description).ToList()
               };
           }

           return new AuthResult
           {
               Succeeded = true,
               Email = user.Email,
           };
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error during email verification for {Email}", email);
           return new AuthResult
           {
               Succeeded = false,
               Errors = new List<string> { "An error occurred during email verification." }
           }; 
       }
   }

   /// <summary>
   /// Receives user email to start with forgot password process
   /// </summary>
   /// <param name="email"></param>
   /// <returns>An AuthResult object</returns>
   public async Task<AuthResult> ForgotPasswordAsync(string email)
   {
       try
       {
           if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email), "Email is required.");
           
           var user = await _userManager.FindByEmailAsync(email);
           if (user == null)
           {
               return new AuthResult
               {
                   Succeeded = false,
                   Errors = new List<string> { "User not found." }
               };
           }
           
           var token = await _userManager.GeneratePasswordResetTokenAsync(user);
           var resetLinkCallbackUrl =
               $"/reset-password?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";

           await _emailService.SendPasswordResetEmailAsync(user.Email, resetLinkCallbackUrl);
           
           return new AuthResult { Succeeded = true };
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error during forgot password for {Email}", email);
           return new AuthResult
           {
               Succeeded = false,
               Errors = new List<string> { "An error occurred while processing your request." }
           }; 
       }
   }

   /// <summary>
   /// Reset current user password with new password
   /// </summary>
   /// <param name="resetPasswordDto">Reset password DTO input</param>
   /// <returns>An AuthResult object</returns>
   public async Task<AuthResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
   {
       try
       {
           if (resetPasswordDto == null) throw new ArgumentNullException(nameof(resetPasswordDto));

           var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
           if (user == null)
           {
               return new AuthResult
               {
                   Succeeded = false,
                   Errors = new List<string> { "User not found." }
               };
           }

           var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
           if (!result.Succeeded)
           {
               return new AuthResult
               {
                   Succeeded = false,
                   Errors = result.Errors.Select(e => e.Description).ToList()
               };
           }

           await _tokenService.RevokeTokenAsync(user.Id);
           
           return new AuthResult { Succeeded = true };
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error during password reset for {Email}", resetPasswordDto?.Email);
           return new AuthResult
           {
               Succeeded = false,
               Errors = new List<string> { "An error occurred while resetting your password." }
           }; 
       }
   }
}