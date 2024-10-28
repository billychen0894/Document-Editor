namespace CollabDocumentEditor.Core.Dtos.AuthDtos;

public class AuthResult
{
    public bool Succeeded { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public IEnumerable<string> Errors { get; set; }
}