namespace CollabDocumentEditor.Core.Settings;

public class JwtSettings
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpirationHours { get; set; }
    public int RefreshTokenExpirationDays { get; set; }
    
    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Key) 
               && !string.IsNullOrEmpty(Issuer) 
               && !string.IsNullOrEmpty(Audience)
               && AccessTokenExpirationHours > 0
               && RefreshTokenExpirationDays > 0;
    }
}