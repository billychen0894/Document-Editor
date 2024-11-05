using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Enum;

namespace CollabDocumentEditor.Core.Interfaces.Repositories;

public interface IDocumentPermissionRepository
{
   Task<DocumentUserPermission?> GetUserPermissionAsync(Guid documentId, Guid userId); 
   Task<IEnumerable<DocumentUserPermission>> GetDocumentPermissionsAsync(Guid documentId);
   Task<List<DocumentUserPermission>> GetUserPermissionsAsync(Guid userId);
   Task<DocumentUserPermission> GrantPermissionAsync(DocumentUserPermission permission);
   Task RevokePermissionAsync(Guid documentId, Guid userId, Guid revokedBy);
   Task<bool> HasPermissionAsync(Guid documentId, Guid userId, DocumentRole permission);
}