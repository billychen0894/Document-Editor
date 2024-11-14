
using CollabDocumentEditor.Core.Entities;

namespace CollabDocumentEditor.UnitTests.Builders;

public class DocumentBuilder
{
    private readonly Document _document;
    
    public DocumentBuilder()
    {
        _document = new Document
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Content = "Test Content",
            UserId = Guid.NewGuid()
        };
    }
    
    public DocumentBuilder WithId(Guid id)
    {
        _document.Id = id;
        return this;
    }
    
    
    public DocumentBuilder WithUserId(Guid userId)
    {
        _document.UserId = userId;
        return this;
    }
    
    public Document Build() => _document;
}