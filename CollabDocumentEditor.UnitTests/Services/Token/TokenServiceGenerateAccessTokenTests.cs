using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;

namespace CollabDocumentEditor.UnitTests.Services.Token;

public class TokenServiceGenerateAccessTokenTests : TokenServiceTestsBase
{
   [Fact]
   public void GenerateAccessToken_WithValidUser_ReturnsValidToken()
   {
      // Arrange
      var user = new ApplicationUserBuilder().Build();

      // Act
      var token = TokenService.GenerateAccessToken(user);
      
      // Assert
      Assert.NotNull(token);
      var handler = new JwtSecurityTokenHandler();
      var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
      
      Assert.NotNull(jsonToken);
      Assert.Equal(JwtSettings.Issuer, jsonToken.Issuer);
      Assert.Equal(JwtSettings.Audience, jsonToken.Audiences.First());
      Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.Email && c.Value == user.Email);
      Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
   }
   
}