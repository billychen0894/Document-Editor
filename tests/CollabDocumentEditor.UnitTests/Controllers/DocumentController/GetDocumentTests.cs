using CollabDocumentEditor.Core.Common;
using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.UnitTests.Base;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CollabDocumentEditor.UnitTests.Controllers.DocumentController;

public class GetDocumentTests : DocumentControllerTestsBase
{
    [Fact]
    public async Task GetDocument_WithEmptyGuid_ReturnsBadRequest()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = await DocumentControllerMock.GetDocument(emptyGuid);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task GetDocument_WithValidGuid_ReturnsOkResult()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var value = new DocumentDto
        {
            Id = documentId,
            Title = "Test Document",
            Content = "Test Content",
        };
        var expectedDocument = new Result<DocumentDto>(true,value, "");
        
        DocumentServiceMock
            .Setup(s => s.GetDocumentAsync(documentId))
            .ReturnsAsync(expectedDocument);

        // Act
        var result = await DocumentControllerMock.GetDocument(documentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDocument = Assert.IsType<Result<DocumentDto>>(okResult.Value);
        Assert.Equal(expectedDocument.Value.Id, returnedDocument.Value.Id);
    } 
}