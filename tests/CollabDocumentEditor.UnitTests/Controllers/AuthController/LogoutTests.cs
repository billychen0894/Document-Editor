using CollabDocumentEditor.UnitTests.Base;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CollabDocumentEditor.UnitTests.Controllers.AuthController;

public class LogoutTests : AuthControllerTestsBase
{
    [Fact]
    public async Task Logout_WithValidUserId_ReturnsOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        AuthServiceMock
            .Setup(s => s.LogoutAsync(userId))
            .ReturnsAsync(true);

        // Act
        var result = await AuthControllerMock.Logout(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("User logged out", okResult.Value);
    }

    [Fact]
    public async Task Logout_WithEmptyUserId_ReturnsBadRequest()
    {
        // Arrange
        var userId = Guid.Empty;

        // Act
        var result = await AuthControllerMock.Logout(userId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("User id cannot be empty", badRequestResult.Value);
    } 
}