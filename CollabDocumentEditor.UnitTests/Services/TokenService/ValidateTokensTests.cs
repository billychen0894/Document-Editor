using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using CollabDocumentEditor.UnitTests.Helpers.Auth;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.TokenService;

public class ValidateTokensTests : TokenServiceTestsBase
{
   [Fact]
   public async Task ValidateAccessToken_WithValidToken_ReturnsSuccessfulTokenResult()
   {
      // Arrange
      var user = new ApplicationUserBuilder().Build();

      UserManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
         .ReturnsAsync(user);
      
      var accessToken = TokenService.GenerateAccessToken(user);
      
      // Act
      var result = await TokenService.ValidateAccessTokenAsync(accessToken);
      
      // Assert
      TokenAssertions.AssertionSuccessfulTokenResult(result);
   }
   
   [Fact]
   public async Task ValidateRefreshToken_WithValidToken_ReturnsSuccessfulTokenResult()
   {
      // Arrange
      var user = new ApplicationUserBuilder().Build();
      
      UserManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
         .ReturnsAsync(user);
      
      var accessToken = TokenService.GenerateAccessToken(user);
      
      // Act
      var result = await TokenService.ValidateRefreshTokenAsync(accessToken, user.RefreshToken);
      
      // Assert
      TokenAssertions.AssertionSuccessfulTokenResult(result);
   }
}