using CollabDocumentEditor.Core.Enum;

namespace CollabDocumentEditor.Core.Authorization;

public class DocumentRoleRequirement : DocumentRequirement
{
   public DocumentRole DocumentRole { get; }
   
   public DocumentRoleRequirement(DocumentRole documentRole)
   {
       DocumentRole = documentRole;
   }
}