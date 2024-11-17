using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CollabDocumentEditor.UnitTests.Controllers.AuthController;

public class LoginTests : AuthControllerTestsBase
{
    [Fact]
    public async Task Login_WhenSuccessful_ReturnsOkResult()
    {
        // Arrange
        var loginDto = new LoginDtoBuilder().Build();
        var expectedResult = new AuthResultBuilder().Build(); 

        AuthServiceMock
            .Setup(s => s.LoginAsync(loginDto))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await AuthControllerMock.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<AuthResult>(okResult.Value);
        Assert.True(returnedResult.Succeeded);
        Assert.Equal(expectedResult.AccessToken, returnedResult.AccessToken);
    }

    [Fact]
    public async Task Login_WhenFails_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginDtoBuilder()
            .WithPassword("wrong")
            .Build();
        var expectedResult = new AuthResultBuilder()
            .WithSucceeded(false)
            .WithErrors(new[] { "Invalid password" })
            .Build();

        AuthServiceMock
            .Setup(s => s.LoginAsync(loginDto))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await AuthControllerMock.Login(loginDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    } 
}