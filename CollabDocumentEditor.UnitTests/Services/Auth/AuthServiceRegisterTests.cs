using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using CollabDocumentEditor.UnitTests.Constants;
using CollabDocumentEditor.UnitTests.Helpers.Auth;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.Auth;

public class AuthServiceRegisterTests : AuthServiceTestsBase
{
    [Fact]
    public async Task RegisterAsync_WithValidInput_ReturnsSuccessResult()
    {
        // Arrange
        var registerDto = new RegisterDtoBuilder().Build();
        var newUser = new ApplicationUserBuilder()
            .WithEmail(registerDto.Email)
            .Build();

        SetupSuccessfulRegistration(registerDto, newUser);

        // Act
        var result = await AuthService.RegisterAsync(registerDto);

        // Assert
        AuthAssertions.AssertionSuccessfulAuthResult(result, newUser);
        VerifySuccessfulRegistration(newUser, registerDto.Password);
    } 
    
    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ReturnsFailure()
    {
        // Arrange
        var registerDto = new RegisterDtoBuilder().Build();
        var existingUser = new ApplicationUserBuilder()
            .WithEmail(registerDto.Email)
            .Build();

        MapperMock.Setup(x => x.Map<RegisterDto, ApplicationUser>(It.IsAny<RegisterDto>()))
            .Returns(existingUser);
        
        UserManagerMock.Setup(x => x.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync(existingUser);

        RegisterValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<RegisterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // Act
        var result = await AuthService.RegisterAsync(registerDto);

        // Assert
        AuthAssertions.AssertionFailedAuthResult(result, "User with this email already exists.");
        UserManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()), Times.Never);
    }
    
    private void SetupSuccessfulRegistration(RegisterDto dto, ApplicationUser user)
    {
        RegisterValidatorMock.Setup(x => x.ValidateAsync(It.IsAny<RegisterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        MapperMock.Setup(x => x.Map<RegisterDto, ApplicationUser>(It.IsAny<RegisterDto>()))
            .Returns(user);

        UserManagerMock.Setup(x => x.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser)null!);

        UserManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);

        UserManagerMock.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        UserManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync("email_confirmation_token");

        TokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<ApplicationUser>()))
            .Returns(TestConstants.AuthConstants.ValidAccessToken);

        TokenServiceMock.Setup(x => x.GenerateRefreshToken())
            .Returns(TestConstants.AuthConstants.ValidRefreshToken);
    }

    private void VerifySuccessfulRegistration(ApplicationUser user, string password)
    {
        UserManagerMock.Verify(x => x.CreateAsync(It.IsAny<ApplicationUser>(), password), Times.Once);
        UserManagerMock.Verify(x => x.UpdateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        TokenServiceMock.Verify(x => x.GenerateAccessToken(It.Is<ApplicationUser>(u => u.Email == user.Email)), Times.Once);
        TokenServiceMock.Verify(x => x.GenerateRefreshToken(), Times.Once);
        EmailServiceMock.Verify(x => x.SendEmailConfirmationAsync(
            It.Is<string>(e => e == user.Email),
            It.IsAny<string>()), Times.Once);
    }
}