using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Enum;
using CollabDocumentEditor.Core.Interfaces.Repositories;
using CollabDocumentEditor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CollabDocumentEditor.Infrastructure.Repositories;

public class DocumentPermissionRepository : IDocumentPermissionRepository
{
   private readonly ApplicationDbContext _context;
   private readonly ILogger<DocumentPermissionRepository> _logger;

   public DocumentPermissionRepository(ApplicationDbContext context, ILogger<DocumentPermissionRepository> logger)
   {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
   }

   /// <summary>
   /// Fetches the user permission for a document
   /// </summary>
   public async Task<DocumentUserPermission?> GetUserPermissionAsync(Guid documentId, Guid userId)
   {
      try
      {
         return await _context.DocumentUserPermissions
            .FirstOrDefaultAsync(p => 
               p.DocumentId == documentId
               && p.UserId == userId
               && p.RevokedAt == null);
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error occurred while fetching user permission for document {DocumentId} and user {UserId}", documentId, userId);
         throw;
      }
   }

   /// <summary>
   /// Fetches all permissions for a document
   /// </summary>
   public async Task<IEnumerable<DocumentUserPermission>> GetDocumentPermissionsAsync(Guid documentId)
   {
      try
      {
         return await _context.DocumentUserPermissions
            .Include(p => p.User)
            .Where(p => p.DocumentId == documentId && p.RevokedAt == null)
            .OrderBy(p => p.Role)
            .ToListAsync();
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error occurred while fetching permissions for document {DocumentId}", documentId);
         throw;
      }
   }

   /// <summary>
   /// Fetches all permissions of documents for a user
   /// </summary>
   /// <param name="userId"></param>
   /// <returns></returns>
   public async Task<List<DocumentUserPermission>> GetUserPermissionsAsync(Guid userId)
   {
      try
      {
         return await _context.DocumentUserPermissions
            .Include(p => p.Document)
            .Where(p => p.UserId == userId && p.RevokedAt == null)
            .OrderBy(p => p.Document.Title)
            .ToListAsync();
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error occurred while fetching permissions for user {UserId}", userId);
         throw;
      }
   }

   /// <summary>
   /// Grants permission to a user for a document
   /// </summary>
   public async Task<DocumentUserPermission> GrantPermissionAsync(DocumentUserPermission permission)
   {
      try
      {
         // Revoke existing permission if any
         await RevokePermissionAsync(permission.DocumentId, permission.UserId, permission.GrantedBy);

         // Grant new permission
         await _context.DocumentUserPermissions.AddAsync(permission);
         await _context.SaveChangesAsync();

         _logger.LogInformation(
            "Permission granted for document {DocumentId} and user {UserId} by {GrantedBy}",
            permission.DocumentId, permission.UserId, permission.GrantedBy);
         
         return permission;
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error occurred while granting permission for document {DocumentId} and user {UserId}", permission.DocumentId, permission.UserId);
         throw;
      }
   }

   /// <summary>
   /// Revokes permission for a user on a document
   /// </summary>
   public async Task RevokePermissionAsync(Guid documentId, Guid userId, Guid revokedBy)
   {
      try
      {
         var existingPermission = await GetUserPermissionAsync(documentId, userId);
         if (existingPermission != null)
         {
            existingPermission.RevokedAt = DateTime.UtcNow;
            existingPermission.RevokedBy = revokedBy;
            
            await _context.SaveChangesAsync();
            _logger.LogInformation(
               "Permission revoked for document {DocumentId} and user {UserId} by {RevokedBy}",
               documentId, userId, revokedBy);
         }
      } 
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error occurred while revoking permission for document {DocumentId} and user {UserId}", documentId, userId);
         throw;
      }
   }

   /// <summary>
   /// Checks if a user has a specific permission on a document
   /// </summary>
   public async Task<bool> HasPermissionAsync(Guid documentId, Guid userId, DocumentRole role)
   {
      try
      {
         // Check if user is the owner
         var isOwner = await _context.Documents.AnyAsync(d => d.Id == documentId && d.UserId == userId);
         if (isOwner)
         {
            return true;
         }
         
         // Check if user has the required permission
         var userPermission = await GetUserPermissionAsync(documentId, userId);
         return userPermission is { RevokedAt: not null } && userPermission.Role <= role;
      }
      catch (Exception ex)
      {
         _logger.LogError(ex, "Error occurred while checking permission for document {DocumentId} and user {UserId}", documentId, userId);
         throw;
      }
   }
}