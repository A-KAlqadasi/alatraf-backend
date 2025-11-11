using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.AssignRepairCardToDoctor;

public class AssignRepairCardToDoctorCommandValidator : AbstractValidator<AssignRepairCardToDoctorCommand>
{
    public AssignRepairCardToDoctorCommandValidator()
    {
        RuleFor(x => x.RepairCardId)
            .GreaterThan(0).WithMessage("Repair card is invalid");
        RuleFor(x => x.DoctorSectionRoomId)
            .GreaterThan(0).WithMessage("Doctor section Id is invalid");
    }
}