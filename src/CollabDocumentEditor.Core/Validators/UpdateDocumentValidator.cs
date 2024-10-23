using CollabDocumentEditor.Core.Dtos;
using FluentValidation;

namespace CollabDocumentEditor.Core.Validators;

public class UpdateDocumentValidator : AbstractValidator<UpdateDocumentDto>
{
    public UpdateDocumentValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Document Id is required.");
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .MinimumLength(1)
            .WithMessage("Document Title must be between 1 and 200 characters.");
        
        RuleFor(x => x.Content)
            .NotEmpty()
            .When(x => x.Content != null)
            .WithMessage("If provided, the content is required.");
    }
}