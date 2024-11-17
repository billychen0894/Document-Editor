using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CollabDocumentEditor.UnitTests.Controllers.AuthController;

public class ForgotPasswordTests : AuthControllerTestsBase
{
    [Fact]
    public async Task ForgotPassword_WithValidEmail_ReturnsOkResult()
    {
        // Arrange
        var email = "test@example.com";
        var expectedResult = new AuthResultBuilder().Build();

        AuthServiceMock
            .Setup(s => s.ForgotPasswordAsync(email))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await AuthControllerMock.ForgotPassword(email);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<AuthResult>(okResult.Value);
        Assert.True(returnedResult.Succeeded);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task ForgotPassword_WithInvalidEmail_ReturnsBadRequest(string email)
    {
        // Act
        var result = await AuthControllerMock.ForgotPassword(email);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Email cannot be empty", badRequestResult.Value);
    } 
}