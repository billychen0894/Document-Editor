using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CollabDocumentEditor.UnitTests.Controllers.AuthController;

public class RegisterTests : AuthControllerTestsBase
{
    [Fact]
    public async Task Register_WhenSuccessful_ReturnsOkResult()
    {
        // Arrange
        var registerDto = new RegisterDtoBuilder().Build(); 
        var expectedResult = new AuthResult 
        { 
            Succeeded = true,
            UserId = Guid.NewGuid(),
            Email = registerDto.Email
        };

        AuthServiceMock
            .Setup(s => s.RegisterAsync(registerDto))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await AuthControllerMock.Register(registerDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedResult = Assert.IsType<AuthResult>(okResult.Value);
        Assert.True(returnedResult.Succeeded);
        Assert.Equal(expectedResult.UserId, returnedResult.UserId);
    }

    [Fact]
    public async Task Register_WhenFails_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterDtoBuilder()
            .WithPassword("weak")
            .WithConfirmPassword("weak")
            .Build();
        var expectedResult = new AuthResult 
        { 
            Succeeded = false,
            Errors = new[] { "Password is too weak" }
        };

        AuthServiceMock
            .Setup(s => s.RegisterAsync(registerDto))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await AuthControllerMock.Register(registerDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    } 
}