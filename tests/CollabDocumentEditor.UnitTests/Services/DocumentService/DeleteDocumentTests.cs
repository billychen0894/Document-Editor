using CollabDocumentEditor.Core.Enum;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.DocumentService;

public class DeleteDocumentTests : DocumentServiceTestsBase
{
    [Fact]
    public async Task DeleteDocumentAsync_UserIsOwner_DeletesDocument()
    {
        // Arrange
        var document = new DocumentBuilder().Build();

        DocumentRepositoryMock.Setup(x => x.GetByIdAsync(document.Id))
            .ReturnsAsync(document);
        DocumentPermissionRepositoryMock.Setup(x => x.HasPermissionAsync(document.Id, document.UserId, DocumentRole.Owner))
            .ReturnsAsync(true);
        CurrentUserServiceMock.Setup(x => x.UserId).Returns(document.UserId);

        // Act
        await DocumentServiceMock.DeleteDocumentAsync(document.Id);

        // Assert
        DocumentRepositoryMock.Verify(x => x.DeleteAsync(document.Id), Times.Once);
    } 
    
    [Fact]
    public async Task DeleteDocumentAsync_UserIsNotOwner_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var document = new DocumentBuilder().Build();

        DocumentRepositoryMock.Setup(x => x.GetByIdAsync(document.Id))
            .ReturnsAsync(document);
        DocumentPermissionRepositoryMock.Setup(x => x.HasPermissionAsync(document.Id, document.UserId, DocumentRole.Owner))
            .ReturnsAsync(false);
        CurrentUserServiceMock.Setup(x => x.UserId).Returns(document.UserId);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => DocumentServiceMock.DeleteDocumentAsync(document.Id));
    }
}