using System.Security.Claims;
using CollabDocumentEditor.Core.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CollabDocumentEditor.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CurrentUserService> _logger;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Guid UserId
    {
        get
        {
            try
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("User ID claim not found in the current context");
                    throw new UnauthorizedAccessException("User is not authenticated");
                }

                if (Guid.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }
               
                _logger.LogError("Invalid user ID format in claims: {UserIdClaim}", userIdClaim);
                throw new InvalidOperationException("Invalid user ID format in claims");
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException && ex is not InvalidOperationException)
            {
                _logger.LogError(ex, "Error retrieving user ID from claims");
                throw new InvalidOperationException("Error retrieving user ID from claims", ex); 
            }
        }
    }

    public bool IsAdmin
    {
        get
        {
            try
            {
                var isAdmin = _httpContextAccessor.HttpContext?.User.IsInRole("Admin") ?? false;

                if (isAdmin)
                {
                    _logger.LogInformation("User {UserId} accessed with admin privileges", UserId);
                }
                
                return isAdmin;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking admin status for user");
                return false; 
            }
        }
    }

    public string UserName
    {
        get
        {
            try
            {
                var userName = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(userName))
                {
                    _logger.LogWarning("Username claim not found for user {UserId}", UserId);
                    return string.Empty;
                }
                
                return userName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving username from claims");
                return string.Empty;
            }
        }
    }

    public IEnumerable<string> Roles
    {
        get
        {
            try
            {
                var roles = _httpContextAccessor.HttpContext?.User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList() ?? new List<string>();

                if (roles.Count == 0)
                {
                    _logger.LogDebug("No roles found for user {UserId}", UserId);
                }
                
                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles from claims");
                return Enumerable.Empty<string>(); 
            }
        }
    }
}