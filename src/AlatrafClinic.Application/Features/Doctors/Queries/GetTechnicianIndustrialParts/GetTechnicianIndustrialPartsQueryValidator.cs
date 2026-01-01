using FluentValidation;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianIndustrialParts;

public class GetTechnicianIndustrialPartsQueryValidator : AbstractValidator<GetTechnicianIndustrialPartsQuery>
{
    public GetTechnicianIndustrialPartsQueryValidator()
    {
        RuleFor(x => x.DoctorSectionRoomId)
            .GreaterThan(0).WithMessage("DoctorSectionRoomId must be greater than 0.");
    }
}