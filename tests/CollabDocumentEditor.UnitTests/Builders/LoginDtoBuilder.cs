using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.UnitTests.Constants;

namespace CollabDocumentEditor.UnitTests.Builders;

public class LoginDtoBuilder
{
    private readonly LoginDto _dto;
    
    public LoginDtoBuilder()
    {
        _dto = new LoginDto
        {
            Email = TestConstants.AuthConstants.ValidEmail,
            Password = TestConstants.AuthConstants.ValidPassword
        };
    }
    
    public LoginDtoBuilder WithEmail(string email)
    {
        _dto.Email = email;
        return this;
    }
    
    public LoginDtoBuilder WithPassword(string password)
    {
        _dto.Password = password;
        return this;
    }
    
    public LoginDto Build()
    {
        return _dto;
    }
}