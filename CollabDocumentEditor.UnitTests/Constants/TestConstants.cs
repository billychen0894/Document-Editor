namespace CollabDocumentEditor.UnitTests.Constants;

public class TestConstants
{
    public static class AuthConstants
    {
        public const string ValidEmail = "test@example.com";
        public const string ValidPassword = "Password123!";
        public const string ValidFirstName = "John";
        public const string ValidLastName = "Doe";
        public const string ValidAccessToken = "valid_access_token";
        public const string ValidRefreshToken = "valid_refresh_token";
        public static readonly Guid ValidUserId = Guid.NewGuid();
    }
}