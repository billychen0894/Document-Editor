using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CollabDocumentEditor.UnitTests.Controllers.AuthController;

public class ResetPasswordTests : AuthControllerTestsBase
{
    [Fact]
    public async Task ResetPassword_WithValidInput_ReturnsOkResult()
    {
        // Arrange
        var email = "test@example.com";
        var token = "valid-token";
        var resetPasswordRequestDto = new ResetPasswordRequestDtoBuilder().Build();
        var expectedResult = new AuthResultBuilder().Build();

        AuthServiceMock
            .Setup(s => s.ResetPasswordAsync(It.IsAny<ResetPasswordDto>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await AuthControllerMock.ResetPassword(email, token, resetPasswordRequestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<AuthResult>(okResult.Value);
        Assert.True(returnedResult.Succeeded);
    }

    [Theory]
    [InlineData(null, "token")]
    [InlineData("", "token")]
    [InlineData("email@test.com", null)]
    [InlineData("email@test.com", "")]
    public async Task ResetPassword_WithInvalidInput_ReturnsBadRequest(string email, string token)
    {
        // Arrange
        var resetPasswordRequestDto = new ResetPasswordRequestDtoBuilder().Build();

        // Act
        var result = await AuthControllerMock.ResetPassword(email, token, resetPasswordRequestDto);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Email or token cannot be empty", badRequestResult.Value);
    } 
}