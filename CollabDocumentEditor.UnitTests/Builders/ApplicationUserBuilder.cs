using CollabDocumentEditor.Core.Entities;
using CollabDocumentEditor.UnitTests.Constants;

namespace CollabDocumentEditor.UnitTests.Builders;

public class ApplicationUserBuilder
{
    private readonly ApplicationUser _user;
    
    public ApplicationUserBuilder()
    {
        _user = new ApplicationUser
        {
            Id = TestConstants.AuthConstants.ValidUserId,
            Email = TestConstants.AuthConstants.ValidEmail,
            UserName = TestConstants.AuthConstants.ValidEmail,
            FirstName = TestConstants.AuthConstants.ValidFirstName,
            LastName = TestConstants.AuthConstants.ValidLastName
        };
    }
    
    public ApplicationUserBuilder WithEmail(string email)
    {
        _user.Email = email;
        _user.UserName = email;
        return this;
    }
    
    public ApplicationUserBuilder WithId(Guid id)
    {
        _user.Id = id;
        return this;
    }
    
    public ApplicationUser Build()
    {
        return _user;
    }
}