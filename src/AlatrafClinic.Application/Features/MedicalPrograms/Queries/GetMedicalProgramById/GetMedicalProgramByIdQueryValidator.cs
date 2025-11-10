using FluentValidation;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalProgramById;

public class GetMedicalProgramByIdQueryValidator : AbstractValidator<GetMedicalProgramByIdQuery>
{
    public GetMedicalProgramByIdQueryValidator()
    {
        RuleFor(x => x.MedicalProgramId)
            .GreaterThan(0).WithMessage("Medical Program Id must be greater than zero.");
    }
}