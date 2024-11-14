using CollabDocumentEditor.Core.Common;
using CollabDocumentEditor.Core.Dtos;

namespace CollabDocumentEditor.UnitTests.Helpers.Document;

public class DocumentAssertions
{
    public static void AssertionSuccessfulDocumentResult(Result<DocumentDto> result, Core.Entities.Document document)
    {
        Assert.True(result.IsSuccess);
        Assert.Equal(document.Id, result.Value.Id);
        Assert.Equal(document.UserId, result.Value.UserId);
        Assert.Equal(document.Title, result.Value.Title);
        Assert.Equal(document.Content, result.Value.Content);
        Assert.Equal(document.CreatedAt, result.Value.CreatedAt);
        Assert.Equal(document.UpdatedAt, result.Value.UpdatedAt);
        Assert.Empty(result.Error);
    }
    
    public static void AssertionFailedDocumentResult(Result<DocumentDto> result, string expectedError)
    {
        Assert.False(result.IsSuccess);
        Assert.Contains(expectedError, result.Error);
    }
    
    public static void AssertionSuccessfulDocumentsResult(Result<IEnumerable<DocumentDto>> result, IEnumerable<Core.Entities.Document> documents)
    {
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Error);
        
        var documentsList = documents.ToList();
        var resultDocumentsList = result.Value.ToList();
        Assert.Equal(documentsList.Count, resultDocumentsList.Count);
        
        for (var i = 0; i < documentsList.Count; i++)
        {
            Assert.Equal(documentsList[i].Id, resultDocumentsList[i].Id);
            Assert.Equal(documentsList[i].UserId, resultDocumentsList[i].UserId);
            Assert.Equal(documentsList[i].Title, resultDocumentsList[i].Title);
            Assert.Equal(documentsList[i].Content, resultDocumentsList[i].Content);
            Assert.Equal(documentsList[i].CreatedAt, resultDocumentsList[i].CreatedAt);
            Assert.Equal(documentsList[i].UpdatedAt, resultDocumentsList[i].UpdatedAt);
        }
    }
    
    public static void AssertionFailedDocumentsResult(Result<IEnumerable<DocumentDto>> result, string expectedError)
    {
        Assert.False(result.IsSuccess);
        Assert.Contains(expectedError, result.Error);
    }
}