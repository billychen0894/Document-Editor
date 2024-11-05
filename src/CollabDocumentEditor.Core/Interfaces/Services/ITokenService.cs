using System.Security.Claims;
using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.Core.Dtos.TokenDtos;
using CollabDocumentEditor.Core.Entities;

namespace CollabDocumentEditor.Core.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(ApplicationUser user);
    string GenerateRefreshToken();
    Task<TokenValidationResult> ValidateAccessTokenAsync(string accessToken);
    Task<TokenValidationResult> ValidateRefreshTokenAsync(string accessToken, string refreshToken);
    Task<AuthResult> RefreshTokenAsync(string accessToken, string refreshToken);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
    Task<bool> RevokeTokenAsync(Guid userId);
} 