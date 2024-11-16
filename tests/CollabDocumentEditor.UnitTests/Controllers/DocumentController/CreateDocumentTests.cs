using CollabDocumentEditor.Core.Common;
using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.UnitTests.Base;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CollabDocumentEditor.UnitTests.Controllers.DocumentController;

public class CreateDocumentTests : DocumentControllerTestsBase
{
    [Fact]
    public async Task CreateDocument_ValidDto_ReturnsCreatedAtRoute()
    {
        // Arrange
        var createDto = new CreateDocumentDto { Title = "Test Document", Content = "Test Content" };
        var resultDocument = new DocumentDto {Id = Guid.NewGuid(), Title = createDto.Title, Content = createDto.Content};
        var createdDocument = new Result<DocumentDto>(true, resultDocument, "");
        DocumentServiceMock
            .Setup(s => s.CreateDocumentAsync(createDto))
            .ReturnsAsync(createdDocument);

        // Act
        var result = await DocumentControllerMock.CreateDocument(createDto);

        // Assert
        var createdAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
        Assert.Equal("GetDocument", createdAtRouteResult.RouteName);
        Assert.Equal(createdDocument.Value.Id, createdAtRouteResult.RouteValues?["documentId"]);
    } 
}