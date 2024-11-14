using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Helpers.Document;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.DocumentService;

public class GetUserDocuments : DocumentServiceTestsBase
{
    [Fact]
    public async Task GetUserDocumentsAsync_CurrentUser_ReturnsDocuments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var documents = new List<Document> 
        { 
            new Document { Id = Guid.NewGuid() },
            new Document { Id = Guid.NewGuid() }
        };
        
        var documentDtos = documents.Select(d => new DocumentDto { Id = d.Id }).ToList();

        CurrentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        DocumentRepositoryMock.Setup(x => x.GetUserDocumentsAsync(userId))
            .ReturnsAsync(documents);
        MapperMock.Setup(x => x.Map<IEnumerable<DocumentDto>>(documents))
            .Returns(documentDtos);

        // Act
        var result = await DocumentServiceMock.GetUserDocumentsAsync(userId);

        // Assert
        DocumentAssertions.AssertionSuccessfulDocumentsResult(result, documents);
    }  
    
    [Fact]
    public async Task GetUserDocumentsAsync_DifferentUserNotAdmin_ReturnsErrorResult()
    {
        // Arrange
        var currentUserId = Guid.NewGuid();
        var targetUserId = Guid.NewGuid();

        CurrentUserServiceMock.Setup(x => x.UserId).Returns(currentUserId);
        CurrentUserServiceMock.Setup(x => x.IsAdmin).Returns(false);

        var documents = await DocumentServiceMock.GetUserDocumentsAsync(targetUserId);
        // Act & Assert
        DocumentAssertions.AssertionFailedDocumentsResult(documents, "You can only view your own documents");
    }
}