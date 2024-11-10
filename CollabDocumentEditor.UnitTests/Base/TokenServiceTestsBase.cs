using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Settings;
using CollabDocumentEditor.Infrastructure.Services;
using CollabDocumentEditor.UnitTests.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace CollabDocumentEditor.UnitTests.Base;

public class TokenServiceTestsBase
{
    protected readonly JwtSettings JwtSettings;
    protected readonly Mock<UserManager<ApplicationUser>> UserManagerMock;
    protected readonly Mock<ILogger<TokenService>> LoggerMock;
    protected readonly TokenService TokenService;

    protected TokenServiceTestsBase()
    {
        UserManagerMock = CreateUserManagerMock();
        LoggerMock = new Mock<ILogger<TokenService>>();
        JwtSettings = new JwtSettings
        {
            Key = TestConstants.TokenConstants.ValidKey,
            Issuer = TestConstants.TokenConstants.ValidIssuer,
            Audience = TestConstants.TokenConstants.ValidAudience,
            AccessTokenExpirationHours = 1,
            RefreshTokenExpirationDays = 7
        };
        
        TokenService = new TokenService(
            Options.Create(JwtSettings),
            UserManagerMock.Object,
            LoggerMock.Object
        );
    }
    
    private static Mock<UserManager<ApplicationUser>> CreateUserManagerMock()
    {
        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object,
            null, null, null, null, null, null, null, null);
    }
}