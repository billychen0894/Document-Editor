namespace CollabDocumentEditor.Core.Dtos.AuthDtos;

public class ResetPasswordRequestDto
{
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}