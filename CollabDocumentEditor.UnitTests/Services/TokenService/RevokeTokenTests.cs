using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.TokenService;

public class RevokeTokenTests : TokenServiceTestsBase
{
    [Fact]
    public async Task RevokeToken_WithValidUserId_RevokesToken()
    {
        // Arrange
        var existingUser = new ApplicationUserBuilder().Build();

        UserManagerMock.Setup(x => x.FindByIdAsync(existingUser.Id.ToString()))
            .ReturnsAsync(existingUser);

        UserManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        existingUser.RefreshToken = null;
        existingUser.RefreshTokenExpiryTime = DateTime.UtcNow;

        // Act
        var result = await TokenService.RevokeTokenAsync(existingUser.Id);

        // Assert
        Assert.True(result);
    }
}