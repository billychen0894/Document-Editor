using CollabDocumentEditor.Core.Dtos;

namespace CollabDocumentEditor.UnitTests.Builders;

public class DocumentDtoBuilder
{
    private readonly DocumentDto _documentDto;
    
    public DocumentDtoBuilder()
    {
        _documentDto = new DocumentDto
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Content = "Test Content",
            UserId = Guid.NewGuid()
        };
    }
    
    public DocumentDtoBuilder WithId(Guid id)
    {
        _documentDto.Id = id;
        return this;
    }
    
    
    public DocumentDtoBuilder WithUserId(Guid userId)
    {
        _documentDto.UserId = userId;
        return this;
    }
    
    public DocumentDtoBuilder WithTitle(string title)
    {
        _documentDto.Title = title;
        return this;
    }
    
    public DocumentDtoBuilder WithContent(string content)
    {
        _documentDto.Content = content;
        return this;
    }
    
    public DocumentDto Build() => _documentDto;
}