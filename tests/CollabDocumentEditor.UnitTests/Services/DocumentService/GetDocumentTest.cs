using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Enum;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using CollabDocumentEditor.UnitTests.Helpers.Document;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.DocumentService;

public class GetDocumentTest : DocumentServiceTestsBase
{
    [Fact]
    public async Task GetDocumentAsync_WithValidInput_ReturnsSuccessResult()
    {
        // Arrange
        var document = new DocumentBuilder()
            .Build();
        var documentDto = new DocumentDtoBuilder()
            .WithId(document.Id)
            .WithUserId(document.UserId)
            .Build();

        DocumentRepositoryMock.Setup(x => x.GetByIdAsync(document.Id))
            .ReturnsAsync(document);
        
        DocumentPermissionRepositoryMock.Setup(x => x.HasPermissionAsync(document.Id, document.UserId, DocumentRole.Viewer))
            .ReturnsAsync(true);

        CurrentUserServiceMock.Setup(x => x.UserId)
            .Returns(document.UserId);

        MapperMock.Setup(x => x.Map<DocumentDto>(document))
            .Returns(documentDto);

        // Act
        var result = await DocumentServiceMock.GetDocumentAsync(document.Id);

        // Assert
        DocumentAssertions.AssertionSuccessfulDocumentResult(result, document);
    } 
    
    [Fact]
    public async Task GetDocumentAsync_WithInvalidInput_ReturnsFailure()
    {
        // Arrange
        var document = new DocumentBuilder()
            .Build();
        var documentDto = new DocumentDtoBuilder()
            .WithId(document.Id)
            .WithUserId(document.UserId)
            .Build();

        DocumentRepositoryMock.Setup(x => x.GetByIdAsync(document.Id))
            .ReturnsAsync(document);
        
        DocumentPermissionRepositoryMock.Setup(x => x.HasPermissionAsync(document.Id, document.UserId, DocumentRole.Viewer))
            .ReturnsAsync(false);

        CurrentUserServiceMock.Setup(x => x.UserId)
            .Returns(Guid.NewGuid());

        MapperMock.Setup(x => x.Map<DocumentDto>(document))
            .Returns(documentDto);

        // Act
        var result = await DocumentServiceMock.GetDocumentAsync(document.Id);

        // Assert
        DocumentAssertions.AssertionFailedDocumentResult(result, "Error retrieving document");
    }
}