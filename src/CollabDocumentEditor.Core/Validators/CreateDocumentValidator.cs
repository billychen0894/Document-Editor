using CollabDocumentEditor.Core.Dtos;
using FluentValidation;

namespace CollabDocumentEditor.Core.Validators;

public class CreateDocumentValidator : AbstractValidator<CreateDocumentDto>
{
   public CreateDocumentValidator()
   {
      RuleFor(x => x.Title)
         .NotEmpty()
         .MaximumLength(200)
         .MinimumLength(1)
         .WithMessage("Document title must be between 1 and 200 characters.");
      
      RuleFor(x => x.Content)
         .NotEmpty()
         .WithMessage("Document content cannot be empty.");
   } 
}