using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.UnitTests.Constants;

namespace CollabDocumentEditor.UnitTests.Helpers.Auth;

public class AuthAssertions
{
    public static void AssertionSuccessfulAuthResult(AuthResult authResult, ApplicationUser user)
    {
        Assert.True(authResult.Succeeded);
        Assert.Equal(user.Id, authResult.UserId);
        Assert.Equal(user.Email, authResult.Email);
        Assert.NotNull(authResult.AccessToken);
        Assert.NotNull(authResult.RefreshToken);
        Assert.True(authResult.AccessTokenExpiration > DateTime.UtcNow);
        Assert.True(authResult.RefreshTokenExpiration > DateTime.UtcNow);
    }
    
    public static void AssertionFailedAuthResult(AuthResult authResult, string expectedError)
    {
        Assert.False(authResult.Succeeded);
        Assert.Contains(expectedError, authResult.Errors);
    }
}