using CollabDocumentEditor.Core.Dtos.AuthDtos;
using FluentValidation;

namespace CollabDocumentEditor.Core.Validators.AuthValidators;

public class LoginValidator : AbstractValidator<LoginDto>
{
   public LoginValidator()
   {
      RuleFor(x => x.Email)
         .NotEmpty().WithMessage("Email is required.")
         .EmailAddress().WithMessage("Email is invalid.");
      
      RuleFor(x => x.Password)
         .NotEmpty().WithMessage("Password is required.")
         .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
         .Matches("[A-Z]").WithMessage("Password must contain at least one upper case letter.")
         .Matches("[a-z]").WithMessage("Password must contain at least one lower case letter.")
         .Matches("[0-9]").WithMessage("Password must contain numbers.")
         .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");
   } 
}