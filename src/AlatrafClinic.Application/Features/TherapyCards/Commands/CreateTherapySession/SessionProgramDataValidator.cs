using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapySession;

public class SessionProgramDataValidator : AbstractValidator<SessionProgramData>
{
    public SessionProgramDataValidator()
    {
        RuleFor(x => x.DiagnosisProgramId)
            .GreaterThan(0).WithMessage("DiagnosisProgramId must be greater than 0.");

        RuleFor(x => x.DoctorSectionRoomId)
            .GreaterThan(0).WithMessage("DoctorSectionRoomId must be greater than 0.");
    }
}