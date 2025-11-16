using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.ChangeRepairCardStatus;

public class ChangeRepairCardStatusCommandValidator : AbstractValidator<ChangeRepairCardStatusCommand>
{
    public ChangeRepairCardStatusCommandValidator()
    {
        RuleFor(x => x.RepairCardId)
            .GreaterThan(0).WithMessage("RepairCardId must be greater than 0.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid status.");
    }
}