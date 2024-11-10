using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using CollabDocumentEditor.UnitTests.Helpers.Auth;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.TokenService;

public class RefreshTokenTests : TokenServiceTestsBase
{
   [Fact] 
   public async Task RefreshToken_WithValidToken_ReturnsValidToken()
   {
      // Arrange
      var user = new ApplicationUserBuilder().Build();

      UserManagerMock.Setup(x => x.FindByIdAsync(user.Id.ToString()))
         .ReturnsAsync(user);

      UserManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
         .ReturnsAsync(IdentityResult.Success);
      
      var accessToken = TokenService.GenerateAccessToken(user);
      var refreshToken = TokenService.GenerateRefreshToken();
      user.RefreshToken = refreshToken;
      user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(JwtSettings.RefreshTokenExpirationDays);
      
      // Act
      var result = await TokenService.RefreshTokenAsync(accessToken, refreshToken);
      
      // Assert
      AuthAssertions.AssertionSuccessfulAuthResult(result, user);
   }
}