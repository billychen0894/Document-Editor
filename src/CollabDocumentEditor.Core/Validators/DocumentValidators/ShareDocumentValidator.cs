using CollabDocumentEditor.Core.Dtos;
using FluentValidation;

namespace CollabDocumentEditor.Core.Validators.DocumentValidators;

public class ShareDocumentValidator : AbstractValidator<ShareDocumentDto>
{
   public ShareDocumentValidator()
   {
      RuleFor(x => x.DocumentId)
         .NotEmpty()
         .WithMessage("Document Id is required.");
      
      RuleFor(x => x.UserId)
         .NotEmpty()
         .WithMessage("User Id is required.");
      
      RuleFor(x => x.Role)
         .IsInEnum()
         .WithMessage("Role is not valid.");
   }
}