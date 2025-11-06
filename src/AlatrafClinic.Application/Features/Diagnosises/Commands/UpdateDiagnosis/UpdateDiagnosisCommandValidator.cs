using FluentValidation;

namespace AlatrafClinic.Application.Features.Diagnosises.Commands.UpdateDiagnosis;

public class UpdateDiagnosisCommandValidator : AbstractValidator<UpdateDiagnosisCommand>
{
    public UpdateDiagnosisCommandValidator()
    {
        RuleFor(x => x.diagnosisId)
            .GreaterThan(0).WithMessage("Diagnosis ID must be greater than zero.");
        
        RuleFor(x => x.ticketId)
            .GreaterThan(0).WithMessage("Ticket ID must be greater than zero.");    
        
        RuleFor(x => x.diagnosisText)
            .NotEmpty().WithMessage("Diagnosis text is required.")
            .MaximumLength(2000).WithMessage("Diagnosis text cannot exceed 2000 characters.");

        RuleFor(x => x.injuryDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Injury date cannot be in the future.");

        RuleFor(x => x.injuryReasons)
            .NotEmpty().WithMessage("Injury reasons list must contain at least one value.");
        RuleForEach(x => x.injuryReasons)
            .GreaterThan(0).WithMessage("Injury reason IDs must be greater than zero.");

        RuleFor(x => x.injurySides)
            .NotEmpty().WithMessage("Injury sides list must contain at least one value.");
        RuleForEach(x => x.injurySides)
            .GreaterThan(0).WithMessage("Injury side IDs must be greater than zero.");
        
        RuleFor(x => x.injuryTypes)
            .NotEmpty().WithMessage("Injury types list must contain at least one value.");
        RuleForEach(x => x.injuryTypes)
            .GreaterThan(0).WithMessage("Injury type IDs must be greater than zero.");

        RuleFor(x => x.diagnosisType)
           .IsInEnum()
           .WithErrorCode("DiagnosisType_Invalid")
           .WithMessage("Type must be a valid DiagnosisType value.");
    }
}