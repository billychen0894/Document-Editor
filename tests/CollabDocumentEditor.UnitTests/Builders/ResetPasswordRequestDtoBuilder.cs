using CollabDocumentEditor.Core.Dtos.AuthDtos;
using CollabDocumentEditor.UnitTests.Constants;

namespace CollabDocumentEditor.UnitTests.Builders;

public class ResetPasswordRequestDtoBuilder
{
    private readonly ResetPasswordRequestDto _dto;
   
    public ResetPasswordRequestDtoBuilder()
    {
        _dto = new ResetPasswordRequestDto
        {
            NewPassword = TestConstants.AuthConstants.ValidPassword,
            ConfirmPassword = TestConstants.AuthConstants.ValidPassword
        };
    }
    
    public ResetPasswordRequestDto Build()
    {
        return _dto;
    }
}