using CollabDocumentEditor.Core.Entities;

namespace CollabDocumentEditor.Core.Interfaces.Repositories;

public interface IDocumentRepository
{
    Task<Document> GetByIdAsync(Guid documentId);
    Task<IEnumerable<Document>> GetUserDocumentsAsync(Guid userId);
    Task<Document> CreateAsync(Document document);
    Task<Document> UpdateAsync(Document document);
    Task<Document> DeleteAsync(Guid documentId);
}