namespace CollabDocumentEditor.Core.Interfaces.Services;

public interface ICurrentUserService
{
    Guid UserId { get; }
    bool IsAdmin { get; }
    string UserName { get; }
    IEnumerable<string> Roles { get; }
}