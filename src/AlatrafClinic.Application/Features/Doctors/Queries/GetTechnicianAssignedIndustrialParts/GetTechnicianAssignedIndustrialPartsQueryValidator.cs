using FluentValidation;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianAssignedIndustrialParts;

public class GetTechnicianAssignedIndustrialPartsQueryValidator : AbstractValidator<GetTechnicianAssignedIndustrialPartsQuery>
{
    public GetTechnicianAssignedIndustrialPartsQueryValidator()
    {
        RuleFor(x => x.DoctorSectionRoomId)
            .GreaterThan(0).WithMessage("DoctorSectionRoomId must be greater than 0.");
    }
}