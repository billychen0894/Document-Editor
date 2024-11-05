using Microsoft.AspNetCore.Identity;

namespace CollabDocumentEditor.Core.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    // Custom properties
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public virtual ICollection<Document> Documents { get; set; } // Navigation property
}