using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using CollabDocumentEditor.UnitTests.Constants;
using CollabDocumentEditor.UnitTests.Helpers.Auth;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.AuthService;

public class LoginTests : AuthServiceTestsBase
{
    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessResult()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = TestConstants.AuthConstants.ValidEmail,
            Password = TestConstants.AuthConstants.ValidPassword,
        };

        var user = new ApplicationUserBuilder().Build();
            
        SetupSuccessfulLogin(loginDto, user);

        // Act
        var result = await AuthService.LoginAsync(loginDto);

        // Assert
        AuthAssertions.AssertionSuccessfulAuthResult(result, user);
        VerifySuccessfulLogin(user);
    } 
    
    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = TestConstants.AuthConstants.ValidEmail,
            Password = "wrong_password"
        };
        var user = new ApplicationUserBuilder().Build();

        SetupFailedLogin(loginDto, user);

        // Act
        var result = await AuthService.LoginAsync(loginDto);

        // Assert
        AuthAssertions.AssertionFailedAuthResult(result, "Invalid password.");
    }
    
    private void SetupSuccessfulLogin(LoginDto dto, ApplicationUser user)
    {
        // Setup validation
        LoginValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
            
        // Setup user retrieval
        UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        // Setup password check
        SignInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, dto.Password, false))
            .ReturnsAsync(SignInResult.Success);

        // Setup token generation
        TokenServiceMock.Setup(x => x.GenerateAccessToken(user))
            .Returns(TestConstants.AuthConstants.ValidAccessToken);
        TokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns(TestConstants.AuthConstants.ValidRefreshToken);
        
        // Setup user update with refresh token
        UserManagerMock.Setup(x => x.UpdateAsync(It.Is<ApplicationUser>(u => 
                u.Id == user.Id && 
                u.RefreshToken == TestConstants.AuthConstants.ValidRefreshToken)))
            .ReturnsAsync(IdentityResult.Success);
    }
    
    private void VerifySuccessfulLogin(ApplicationUser user)
    {
        SignInManagerMock.Verify(x => x.CheckPasswordSignInAsync(user, TestConstants.AuthConstants.ValidPassword, false), Times.Once);
        UserManagerMock.Verify(x => x.UpdateAsync(It.Is<ApplicationUser>(u => 
                u.Id == user.Id && 
                u.RefreshToken == TestConstants.AuthConstants.ValidRefreshToken)), 
            Times.Once);
        TokenServiceMock.Verify(x => x.GenerateAccessToken(It.Is<ApplicationUser>(u => u.Email == user.Email)), Times.Once);
        TokenServiceMock.Verify(x => x.GenerateRefreshToken(), Times.Once);
    }
    
    private void SetupFailedLogin(LoginDto dto, ApplicationUser user)
    {
        LoginValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<LoginDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
            
        UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        SignInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, dto.Password, false))
            .ReturnsAsync(SignInResult.Failed);
    }
}