using CollabDocumentEditor.Core.Interfaces.Services;
using CollabDocumentEditor.Web.Controllers;
using Microsoft.Extensions.Logging;
using Moq;

namespace CollabDocumentEditor.UnitTests.Base;

public class AuthControllerTestsBase
{
    protected readonly Mock<IAuthService> AuthServiceMock;
    protected readonly Mock<ILogger<AuthController>> LoggerMock;
    protected readonly AuthController AuthControllerMock;

    public AuthControllerTestsBase()
    {
        AuthServiceMock = new Mock<IAuthService>();
        LoggerMock = new Mock<ILogger<AuthController>>();
        AuthControllerMock = new AuthController(AuthServiceMock.Object, LoggerMock.Object);
    } 
}