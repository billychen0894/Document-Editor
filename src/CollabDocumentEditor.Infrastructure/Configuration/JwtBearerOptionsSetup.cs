using System.Text;
using CollabDocumentEditor.Core.Settings;
using Microsoft.IdentityModel.Tokens;

namespace CollabDocumentEditor.Infrastructure.Configuration;

public class JwtBearerOptionsSetup
{
    public static TokenValidationParameters CreateTokenValidationParameters(JwtSettings jwtSettings)
    {
        if (jwtSettings == null)
            throw new ArgumentNullException(nameof(jwtSettings));
        
        if (string.IsNullOrWhiteSpace(jwtSettings.Key))
            throw new InvalidOperationException("JWT Key is not configured.");
        
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ClockSkew = TimeSpan.Zero
        };
    }
}