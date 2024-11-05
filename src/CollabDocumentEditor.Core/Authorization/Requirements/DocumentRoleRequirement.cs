using CollabDocumentEditor.Core.Enum;

namespace CollabDocumentEditor.Core.Authorization.Requirements;

public class DocumentRoleRequirement : DocumentRequirement
{
   public DocumentRole DocumentRole { get; }
   
   public DocumentRoleRequirement(DocumentRole documentRole)
   {
       DocumentRole = documentRole;
   }
}