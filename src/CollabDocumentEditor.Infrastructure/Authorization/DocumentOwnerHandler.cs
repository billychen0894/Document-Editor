using System.Security.Claims;
using CollabDocumentEditor.Core.Authorization;
using CollabDocumentEditor.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace CollabDocumentEditor.Infrastructure.Authorization;

public class DocumentOwnerHandler : AuthorizationHandler<DocumentOwnerRequirement, Document>
{
   protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
      DocumentOwnerRequirement requirement, Document document)
   {
      var user = context.User;

      if (user.Identity?.IsAuthenticated != true) return Task.CompletedTask;

      if (document.UserId == user.FindFirst(ClaimTypes.NameIdentifier)?.Value)
      {
        context.Succeed(requirement); 
      }
      
      return Task.CompletedTask;
   }
}