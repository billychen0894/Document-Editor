using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Exceptions;
using CollabDocumentEditor.Core.Interfaces.Services;
using CollabDocumentEditor.Core.Settings;
using CollabDocumentEditor.Infrastructure.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using TokenValidationResult = CollabDocumentEditor.Core.Dtos.TokenDtos.TokenValidationResult;

namespace CollabDocumentEditor.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<TokenService> _logger;

    public TokenService(IOptions<JwtSettings> jwtSettingsOptions, UserManager<ApplicationUser> userManager, ILogger<TokenService> logger)
    {
        _jwtSettings = jwtSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(jwtSettingsOptions));
        _tokenValidationParameters = JwtBearerOptionsSetup.CreateTokenValidationParameters(_jwtSettings);
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Generates a JWT access token for the specified user 
    /// </summary>
    /// <param name="user">The user identity to extract from for access token generation</param>
    /// <returns>An access token specifically for the user identity</returns>
    /// <exception cref="TokenValidationException">Represents a token validation exception</exception>
    public string GenerateAccessToken(ApplicationUser user)
    {
        try
        {
            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, user.Id),
                new (ClaimTypes.Email, user.Email!),
                new (ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.AccessTokenExpirationHours),
                signingCredentials: credentials
            );
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating access token for user {userId}", user.Id);    
            throw new TokenValidationException("Error generating access token for user", ex);
        }
    }

    /// <summary>
    /// Generates a cryptographically secure refresh token
    /// </summary>
    /// <returns>A cryptographically secure refresh token string</returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Validates an access token and returns the user if valid
    /// </summary>
    /// <param name="accessToken">An accessToken from a user</param>
    /// <returns>TokenValidationResult</returns>
    public async Task<TokenValidationResult> ValidateAccessTokenAsync(string accessToken)
    {
        try
        {
            var principal = GetPrincipalFromToken(accessToken);
            if (principal == null)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Error = "Invalid token"
                };
            }
            
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Error = "User id is missing in the token"
                };
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Error = "User not found"
                };
            }

            return new TokenValidationResult
            {
                IsValid = true,
                User = user,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating access token");
            return new TokenValidationResult
            {
                IsValid = false,
                Error = "Token validation failed"
            };
        }
    }

    /// <summary>
    /// Validates refresh token and its associated access token
    /// </summary>
    /// <param name="accessToken">An access token associated with user</param>
    /// <param name="refreshToken">A refresh token associated with user</param>
    /// <returns>TokenValidationResult</returns>
    public async Task<TokenValidationResult> ValidateRefreshTokenAsync(string accessToken, string refreshToken)
    {
        try
        {
            var principal = GetPrincipalFromToken(accessToken);
            if (principal == null)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Error = "Invalid token"
                };
            }
            
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Error = "User id is missing in the token"
                };
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Error = "User not found"
                };
            }

            if (user.RefreshToken != refreshToken)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Error = "Invalid refresh token"
                };
            }

            if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return new TokenValidationResult
                {
                    IsValid = false,
                    Error = "Refresh token expired"
                };
            }

            return new TokenValidationResult
            {
                IsValid = true,
                User = user,
            };
        }
        catch (Exception ex)
        {
           _logger.LogError(ex, "Error validating refresh token");
           return new TokenValidationResult
           {
               IsValid = false,
               Error = "Token validation failed"
           };
        }
    }

    /// <summary>
    /// Refreshes an access token using a refresh token
    /// </summary>
    /// <param name="accessToken">An access token associated with a user instance</param>
    /// <param name="refreshToken">A refresh token associated with a user instance</param>
    /// <returns>AuthResult</returns>
    public async Task<AuthResult> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        try
        {
            var validationResult = await ValidateRefreshTokenAsync(accessToken, refreshToken);
            if (!validationResult.IsValid)
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = new List<string> { validationResult.Error }
                };
            }
            
            var user = validationResult.User;
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();

            if (string.IsNullOrEmpty(newAccessToken))
            {
                return new AuthResult
                {
                    Succeeded = false,
                    Errors = new List<string> {"Invalid refresh token"}
                };
            }
            
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            await _userManager.UpdateAsync(user);

            return new AuthResult
            {
                Succeeded = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiration = DateTime.UtcNow.AddHours(_jwtSettings.AccessTokenExpirationHours),
                RefreshTokenExpiration = user.RefreshTokenExpiryTime,
                UserId = user.Id,
                Email = user.Email!,
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return new AuthResult
            {
                Succeeded = false,
                Errors = new List<string> { "Error refreshing token" }
            };
        }
    }
    
    /// <summary>
    /// Extracts the ClaimsPrincipal from a JWT token.
    /// </summary>
    /// <param name="token">The JWT token string to extract claims from.</param>
    /// <returns>A ClaimsPrincipal representing the user identity, or null if token is invalid.</returns>
    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Key);

            // Create a clone of the tokenValidationParameters but disable the lifetime validation
            var tokenValidationParameters = _tokenValidationParameters.Clone();
            tokenValidationParameters.ValidateLifetime = false;

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            {
               throw new SecurityTokenException("Invalid token"); 
            }
            
            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting principal from JWT Token");
            return null;
        }
    }

    /// <summary>
    /// Revokes a user's refresh token
    /// </summary>
    /// <param name="userId">A user id associated with a user instance</param>
    /// <returns>true if successfully revoked</returns>
    public async Task<bool> RevokeTokenAsync(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);
            
            return result.Succeeded;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token for user {UserId}", userId);
            return false;
        }
    }
}