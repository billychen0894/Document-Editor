
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

    public static class TokenConstants
    {
        public const string ValidKey = "this_is_a_valid_secret_key_for_testing_purposes";
        public const string ValidIssuer = "valid_issuer";
        public const string ValidAudience = "valid_audience";
        public const int ValidLifetime = 3600;
    }

    public static class EmailConstants
    {
        public const string ValidSendGridApiKey = "valid_sendgrid_api_key";
        public const string ValidFromEmail = "test@example.com";
        public const string ValidFromName = "Test";
        public const int ValidRetryCount = 3;
        public const int ValidRetryDelayMilliseconds = 1000;
        public const bool ValidSandboxMode = false;
        public const string ValidHost = "example.com";
    }
}