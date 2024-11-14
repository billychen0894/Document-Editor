using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Enum;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using CollabDocumentEditor.UnitTests.Helpers.Document;
using FluentValidation.Results;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.DocumentService;

public class UpdateDocumentTests : DocumentServiceTestsBase
{
    [Fact]
    public async Task UpdateDocumentAsync_ValidInputAndPermission_UpdatesDocument()
    {
        // Arrange
        var existingDocument = new DocumentBuilder().Build();
        var updateDto = new UpdateDocumentDto
            { Id = existingDocument.Id, Title = "Updated Title", Content = "Updated Content" };
        var updatedDocument = new Document
        {
            Id = existingDocument.Id, Title = "Updated Title", Content = "Updated Content",
            UserId = existingDocument.UserId
        };
        var documentDto = new DocumentDtoBuilder()
            .WithId(updatedDocument.Id)
            .WithUserId(updatedDocument.UserId)
            .WithTitle(updatedDocument.Title)
            .WithContent(updatedDocument.Content)
            .Build();

        UpdateDocumentValidatorMock.Setup(x => x.ValidateAsync(updateDto, default))
            .ReturnsAsync(new ValidationResult());
        DocumentRepositoryMock.Setup(x => x.GetByIdAsync(existingDocument.Id))
            .ReturnsAsync(existingDocument);
        DocumentPermissionRepositoryMock.Setup(x =>
                x.HasPermissionAsync(existingDocument.Id, existingDocument.UserId, DocumentRole.Editor))
            .ReturnsAsync(true);
        CurrentUserServiceMock.Setup(x => x.UserId).Returns(existingDocument.UserId);
        MapperMock.Setup(x => x.Map<Document>(updateDto))
            .Returns(updatedDocument);
        DocumentRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Document>()))
            .ReturnsAsync(updatedDocument);
        MapperMock.Setup(x => x.Map<DocumentDto>(updatedDocument))
            .Returns(documentDto);

        // Act
        var result = await DocumentServiceMock.UpdateDocumentAsync(updateDto);

        // Assert
        DocumentAssertions.AssertionSuccessfulDocumentResult(result, updatedDocument);
    }
    
    [Fact]
    public async Task UpdateDocumentAsync_InvalidInput_ReturnsFailure()
    {
        // Arrange
        var existingDocument = new DocumentBuilder().Build();
        var updateDto = new UpdateDocumentDto
            { Id = existingDocument.Id, Title = "Updated Title", Content = "Updated Content" };

        UpdateDocumentValidatorMock.Setup(x => x.ValidateAsync(updateDto, default))
            .ReturnsAsync(new ValidationResult());
        DocumentRepositoryMock.Setup(x => x.GetByIdAsync(existingDocument.Id))
            .ReturnsAsync(existingDocument);
        DocumentPermissionRepositoryMock.Setup(x =>
                x.HasPermissionAsync(existingDocument.Id, existingDocument.UserId, DocumentRole.Editor))
            .ReturnsAsync(true);
        CurrentUserServiceMock.Setup(x => x.UserId).Returns(existingDocument.UserId);

        // Act
        var result = await DocumentServiceMock.UpdateDocumentAsync(updateDto);

        // Assert
        DocumentAssertions.AssertionFailedDocumentResult(result, "Error updating document");
    }
}