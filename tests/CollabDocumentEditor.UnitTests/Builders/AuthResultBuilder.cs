using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.UnitTests.Constants;

namespace CollabDocumentEditor.UnitTests.Builders;

public class AuthResultBuilder
{
    private readonly AuthResult _authResult;

    public AuthResultBuilder()
    {
        _authResult = new AuthResult
        {
            Succeeded = true,
            AccessToken = TestConstants.AuthConstants.ValidAccessToken,
            RefreshToken = TestConstants.AuthConstants.ValidRefreshToken,
            AccessTokenExpiration = DateTime.Now.AddHours(1),
            RefreshTokenExpiration = DateTime.Now.AddDays(7),
            UserId = TestConstants.AuthConstants.ValidUserId,
            Email = TestConstants.AuthConstants.ValidEmail,
            Errors = Array.Empty<string>()
        };
    }
    
    public AuthResultBuilder WithSucceeded(bool succeeded)
    {
        _authResult.Succeeded = succeeded;
        return this;
    }
    
    public AuthResultBuilder WithAccessToken(string accessToken)
    {
        _authResult.AccessToken = accessToken;
        return this;
    }
    
    public AuthResultBuilder WithRefreshToken(string refreshToken)
    {
        _authResult.RefreshToken = refreshToken;
        return this;
    }
    
    public AuthResultBuilder WithAccessTokenExpiration(DateTime accessTokenExpiration)
    {
        _authResult.AccessTokenExpiration = accessTokenExpiration;
        return this;
    }
    
    public AuthResultBuilder WithRefreshTokenExpiration(DateTime refreshTokenExpiration)
    {
        _authResult.RefreshTokenExpiration = refreshTokenExpiration;
        return this;
    }
    
    public AuthResultBuilder WithUserId(Guid userId)
    {
        _authResult.UserId = userId;
        return this;
    }
    
    public AuthResultBuilder WithEmail(string email)
    {
        _authResult.Email = email;
        return this;
    }
    
    public AuthResultBuilder WithErrors(IEnumerable<string> errors)
    {
        _authResult.Errors = errors;
        return this;
    }
    
    public AuthResult Build()
    {
        return _authResult;
    }
}