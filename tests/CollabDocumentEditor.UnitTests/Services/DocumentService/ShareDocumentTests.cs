using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Enum;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using FluentValidation.Results;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.DocumentService;

public class ShareDocumentTests : DocumentServiceTestsBase
{
    [Fact]
    public async Task ShareDocumentAsync_ValidInputAndOwnership_SharesDocument()
    {
        // Arrange
        var document = new DocumentBuilder().Build();
        var targetUserId = Guid.NewGuid();
        var shareDto = new ShareDocumentDto
        {
            DocumentId = document.Id,
            UserId = targetUserId,
            Role = DocumentRole.Editor
        };

        ShareDocumentValidatorMock.Setup(x => x.ValidateAsync(shareDto, default))
            .ReturnsAsync(new ValidationResult());
        DocumentRepositoryMock.Setup(x => x.GetByIdAsync(document.Id))
            .ReturnsAsync(document);
        DocumentPermissionRepositoryMock
            .Setup(x => x.HasPermissionAsync(document.Id, document.UserId, DocumentRole.Owner))
            .ReturnsAsync(true);
        CurrentUserServiceMock.Setup(x => x.UserId).Returns(document.UserId);

        // Act
        await DocumentServiceMock.ShareDocumentAsync(shareDto);

        // Assert
        DocumentPermissionRepositoryMock.Verify(x => x.GrantPermissionAsync(
                It.Is<DocumentUserPermission>(p =>
                    p.DocumentId == document.Id &&
                    p.UserId == targetUserId &&
                    p.Role == DocumentRole.Editor &&
                    p.GrantedBy == document.UserId)),
            Times.Once);
    }
    
    [Fact]
    public async Task ShareDocumentAsync_ValidInputAndNotOwnership_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var document = new DocumentBuilder().Build();
        var targetUserId = Guid.NewGuid();
        var shareDto = new ShareDocumentDto
        {
            DocumentId = document.Id,
            UserId = targetUserId,
            Role = DocumentRole.Editor
        };

        ShareDocumentValidatorMock.Setup(x => x.ValidateAsync(shareDto, default))
            .ReturnsAsync(new ValidationResult());
        DocumentRepositoryMock.Setup(x => x.GetByIdAsync(document.Id))
            .ReturnsAsync(document);
        DocumentPermissionRepositoryMock
            .Setup(x => x.HasPermissionAsync(document.Id, document.UserId, DocumentRole.Owner))
            .ReturnsAsync(false);
        CurrentUserServiceMock.Setup(x => x.UserId).Returns(document.UserId);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => DocumentServiceMock.ShareDocumentAsync(shareDto));
    }
}