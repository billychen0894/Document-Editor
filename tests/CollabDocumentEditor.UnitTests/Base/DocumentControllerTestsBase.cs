using CollabDocumentEditor.Core.Interfaces.Services;
using CollabDocumentEditor.Web.Controllers;
using Microsoft.Extensions.Logging;
using Moq;

namespace CollabDocumentEditor.UnitTests.Base;

public class DocumentControllerTestsBase
{
    protected readonly Mock<IDocumentService> DocumentServiceMock;
    protected readonly Mock<ILogger<DocumentsController>> LoggerMock;
    protected readonly DocumentsController DocumentControllerMock;
    
    public DocumentControllerTestsBase()
    {
        DocumentServiceMock = new Mock<IDocumentService>();
        LoggerMock = new Mock<ILogger<DocumentsController>>();
        DocumentControllerMock = new DocumentsController(DocumentServiceMock.Object, LoggerMock.Object);
    }
}