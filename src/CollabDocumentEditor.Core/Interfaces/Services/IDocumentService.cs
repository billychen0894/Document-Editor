using CollabDocumentEditor.Core.Common;
using CollabDocumentEditor.Core.Dtos;

namespace CollabDocumentEditor.Core.Interfaces.Services;

public interface IDocumentService
{
   Task<Result<DocumentDto>> GetDocumentAsync(Guid documentId); 
   Task<Result<IEnumerable<DocumentDto>>> GetUserDocumentsAsync(Guid userId);
   Task<Result<DocumentDto>> CreateDocumentAsync(CreateDocumentDto dto);
   Task<Result<DocumentDto>> UpdateDocumentAsync(UpdateDocumentDto dto);
   Task DeleteDocumentAsync(Guid documentId);
   Task ShareDocumentAsync(ShareDocumentDto dto);
}