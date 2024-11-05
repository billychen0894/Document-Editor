using CollabDocumentEditor.Core.Dtos;

namespace CollabDocumentEditor.Core.Interfaces.Services;

public interface IDocumentService
{
   Task<DocumentDto> GetDocumentAsync(Guid documentId); 
   Task<IEnumerable<DocumentDto>> GetUserDocumentsAsync(Guid userId);
   Task<DocumentDto> CreateDocumentAsync(CreateDocumentDto dto);
   Task<DocumentDto> UpdateDocumentAsync(UpdateDocumentDto dto);
   Task DeleteDocumentAsync(Guid documentId);
   Task ShareDocumentAsync(ShareDocumentDto dto);
}