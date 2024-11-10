using AutoMapper;
using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Interfaces.Services;
using CollabDocumentEditor.Core.Settings;
using CollabDocumentEditor.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CollabDocumentEditor.UnitTests.Base;

public class AuthServiceTestsBase
{
    protected readonly Mock<UserManager<ApplicationUser>> UserManagerMock;
    protected readonly Mock<SignInManager<ApplicationUser>> SignInManagerMock;
    protected readonly Mock<IMapper> MapperMock;
    protected readonly Mock<IValidator<LoginDto>> LoginValidatorMock;
    protected readonly Mock<IValidator<RegisterDto>> RegisterValidatorMock;
    protected readonly Mock<ILogger<AuthService>> LoggerMock;
    protected readonly Mock<ITokenService> TokenServiceMock;
    protected readonly Mock<IEmailService> EmailServiceMock;
    protected readonly JwtSettings JwtSettings;
    protected readonly AuthService AuthService;

    protected AuthServiceTestsBase()
    {
        UserManagerMock = CreateUserManagerMock();
        SignInManagerMock = CreateSignInManagerMock();
        MapperMock = new Mock<IMapper>();
        LoginValidatorMock = new Mock<IValidator<LoginDto>>();
        RegisterValidatorMock = new Mock<IValidator<RegisterDto>>();
        LoggerMock = new Mock<ILogger<AuthService>>();
        TokenServiceMock = new Mock<ITokenService>();
        EmailServiceMock = new Mock<IEmailService>();
        
        JwtSettings = new JwtSettings
        {
            AccessTokenExpirationHours = 1,
            RefreshTokenExpirationDays = 7
        };

        AuthService = new AuthService(
            UserManagerMock.Object,
            SignInManagerMock.Object,
            MapperMock.Object,
            LoginValidatorMock.Object,
            RegisterValidatorMock.Object,
            LoggerMock.Object,
            TokenServiceMock.Object,
            Options.Create(JwtSettings),
            EmailServiceMock.Object
        ); 
    }
    
    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object,
            null, null, null, null, null, null, null, null);
    }

    private Mock<SignInManager<ApplicationUser>> CreateSignInManagerMock()
    {
        return new Mock<SignInManager<ApplicationUser>>(
            UserManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
            null, null, null, null);
    }
}