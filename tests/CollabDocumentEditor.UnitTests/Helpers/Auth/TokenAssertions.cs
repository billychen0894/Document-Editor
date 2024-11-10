using CollabDocumentEditor.Core.Dtos.TokenDtos;

namespace CollabDocumentEditor.UnitTests.Helpers.Auth;

public class TokenAssertions
{
    public static void AssertionSuccessfulTokenResult(TokenValidationResult tokenResult)
    {
        Assert.True(tokenResult.IsValid);
        Assert.Null(tokenResult.Error);
        Assert.NotNull(tokenResult.User);
    }
    
    public static void AssertionFailedTokenResult(TokenValidationResult tokenResult, string expectedError)
    {
        Assert.False(tokenResult.IsValid);
        Assert.Contains(expectedError, tokenResult.Error);
        Assert.Null(tokenResult.User);
    }
}