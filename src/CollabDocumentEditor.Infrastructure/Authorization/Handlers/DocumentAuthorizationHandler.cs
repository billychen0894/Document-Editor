using System.Security.Claims;
using CollabDocumentEditor.Core.Authorization.Requirements;
using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.Core.Enum;
using CollabDocumentEditor.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CollabDocumentEditor.Infrastructure.Authorization.Handlers;

public class DocumentAuthorizationHandler : AuthorizationHandler<DocumentRoleRequirement, Document>
{
    private readonly IDocumentPermissionRepository _permissionRepository;
    private readonly ILogger<DocumentAuthorizationHandler> _logger;

    public DocumentAuthorizationHandler(
        IDocumentPermissionRepository permissionRepository,
        ILogger<DocumentAuthorizationHandler> logger)
    {
        _permissionRepository = permissionRepository;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        DocumentRoleRequirement requirement,
        Document document)
    {
        var userIdString = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        // Admins can access all documents
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        if (userIdString == null)
        {
            _logger.LogWarning("User is not authenticated");
            return;
        }

        // Document owner can access the document
        if (Guid.TryParse(userIdString, out var userId) && document.UserId == userId)
        {
            context.Succeed(requirement);
            return;
        }
        
        // Get user permission for this document
        var permission = await _permissionRepository.GetUserPermissionAsync(document.Id, userId);

        // If user has no permission, they cannot access the document
        if (permission == null || permission.Role == DocumentRole.None || permission.RevokedAt != null)
        {
            _logger.LogWarning("User {UserId} does not have permission to access document {DocumentId}", userId, document.Id);
            return;
        }

        // Check if user's permission is sufficient for the required role
        if (IsRoleSufficient(permission.Role, requirement.DocumentRole))
        {
            context.Succeed(requirement);
            _logger.LogInformation(
                "User {UserId} with role {UserRole} authorized for document {DocumentId} requiring {RequiredRole}",
                userId, permission.Role, document.Id, requirement.DocumentRole);
        }
        else
        {
            _logger.LogWarning(
                "Access denied for user {UserId} with role {UserRole} to document {DocumentId} requiring {RequiredRole}",
                userId, permission.Role, document.Id, requirement.DocumentRole);
        }
    }
    
    private static bool IsRoleSufficient(DocumentRole userRole, DocumentRole requiredRole)
    {
        // Lower enum values have higher privileges
        // Owner (0) > Editor (1) > Viewer (2) > None (3)
        return userRole <= requiredRole;
    }
}