using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.UnitTests.Base;
using Microsoft.AspNetCore.Mvc;

namespace CollabDocumentEditor.UnitTests.Controllers.DocumentController;

public class UpdateDocumentTests : DocumentControllerTestsBase
{
    [Fact]
    public async Task UpdateDocument_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var updateDto = new UpdateDocumentDto { Id = Guid.NewGuid(), Title = "Updated Title", Content = "Updated Content" };

        // Act
        var result = await DocumentControllerMock.UpdateDocument(documentId, updateDto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    } 
}