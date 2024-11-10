using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.UnitTests.Constants;

namespace CollabDocumentEditor.UnitTests.Builders;

public class RegisterDtoBuilder
{
    private readonly RegisterDto _dto;

    public RegisterDtoBuilder()
    {
        _dto = new RegisterDto
        {
            Email = TestConstants.AuthConstants.ValidEmail,
            Password = TestConstants.AuthConstants.ValidPassword,
            ConfirmPassword = TestConstants.AuthConstants.ValidPassword,
            FirstName = TestConstants.AuthConstants.ValidFirstName,
            LastName = TestConstants.AuthConstants.ValidLastName
        };
    }
    
    public RegisterDtoBuilder WithEmail(string email)
    {
        _dto.Email = email;
        return this;
    }
    
    public RegisterDtoBuilder WithPassword(string password)
    {
        _dto.Password = password;
        return this;
    }
    
    public RegisterDtoBuilder WithConfirmPassword(string confirmPassword)
    {
        _dto.ConfirmPassword = confirmPassword;
        return this;
    }
    
    public RegisterDto Build()
    {
        return _dto;
    }
}