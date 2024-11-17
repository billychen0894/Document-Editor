using CollabDocumentEditor.Core.Common;
using CollabDocumentEditor.UnitTests.Base;
using Microsoft.AspNetCore.Mvc;

namespace CollabDocumentEditor.UnitTests.Controllers.DocumentController;

public class DeleteDocumentTests : DocumentControllerTestsBase
{
    [Fact]
    public async Task DeleteDocument_EmptyGuid_ReturnsBadRequest()
    {
        // Arrange
        var emptyGuid = Guid.Empty;

        // Act
        var result = await DocumentControllerMock.DeleteDocument(emptyGuid);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    } 
}