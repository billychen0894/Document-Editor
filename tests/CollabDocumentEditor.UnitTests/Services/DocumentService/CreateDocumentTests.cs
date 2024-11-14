using CollabDocumentEditor.Core.Dtos;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.UnitTests.Base;
using CollabDocumentEditor.UnitTests.Builders;
using CollabDocumentEditor.UnitTests.Helpers.Document;
using FluentValidation.Results;
using Moq;

namespace CollabDocumentEditor.UnitTests.Services.DocumentService;

public class CreateDocumentTests : DocumentServiceTestsBase
{
    [Fact]
    public async Task CreateDocumentAsync_ValidInput_CreatesDocument()
    {
        // Arrange
        var createDto = new CreateDocumentDto { Title = "Test Title", Content = "Test Content" };
        var document = new DocumentBuilder().Build();
        var documentDto = new DocumentDto
        {
            Id = document.Id,
            Title = "Test Title",
            Content = "Test Content",
            UserId = document.UserId
        };

        CreateDocumentValidatorMock.Setup(x => x.ValidateAsync(createDto, default))
            .ReturnsAsync(new ValidationResult());
        MapperMock.Setup(x => x.Map<Document>(createDto))
            .Returns(document);
        CurrentUserServiceMock.Setup(x => x.UserId).Returns(document.UserId);
        DocumentRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Document>()))
            .ReturnsAsync(document);
        MapperMock.Setup(x => x.Map<DocumentDto>(document))
            .Returns(documentDto);

        // Act
        var result = await DocumentServiceMock.CreateDocumentAsync(createDto);

        // Assert
        DocumentAssertions.AssertionSuccessfulDocumentResult(result, document);
    }
    
    [Fact]
    public async Task CreateDocumentAsync_InvalidInput_ReturnsFailure()
    {
        // Arrange
        var createDto = new CreateDocumentDto { Title = "", Content = "" };

        CreateDocumentValidatorMock.Setup(x => x.ValidateAsync(createDto, default))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Title", "Title is required"),
                new("Content", "Content is required")
            }));
        MapperMock.Setup(x => x.Map<Document>(createDto))
            .Returns((Document)null);

        // Act
        var result = await DocumentServiceMock.CreateDocumentAsync(createDto);

        // Assert
        DocumentAssertions.AssertionFailedDocumentResult(result, "Error creating document");
    }
}