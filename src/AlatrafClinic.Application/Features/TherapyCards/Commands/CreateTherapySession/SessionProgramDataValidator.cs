using FluentValidation;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapySession;

public class SessionProgramDataValidator : AbstractValidator<SessionProgramData>
{
    public SessionProgramDataValidator()
    {
        RuleFor(x => x.DiagnosisProgramId)
            .GreaterThan(0).WithMessage("DiagnosisProgramId must be greater than 0.");

        RuleFor(x => x.DocotorId)
            .GreaterThan(0).WithMessage("DoctorId must be greater than 0.");
        RuleFor(x => x.SectionId)
            .GreaterThan(0).WithMessage("SectionId must be greater than 0.");
        RuleFor(x => x.RoomId)
            .GreaterThan(0).WithMessage("RoomId must be greater than 0.");
            
    }
}