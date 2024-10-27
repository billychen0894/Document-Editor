using CollabDocumentEditor.Core.Entities;

namespace CollabDocumentEditor.Core.Dtos.TokenDtos;

public class TokenValidationResult
{
    public bool IsValid { get; set; }
    public string Error  { get; set; }
    public ApplicationUser User { get; set; }
}