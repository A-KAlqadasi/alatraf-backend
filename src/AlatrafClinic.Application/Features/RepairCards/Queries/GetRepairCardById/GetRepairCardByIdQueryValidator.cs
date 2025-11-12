using FluentValidation;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCardById;

public class GetRepairCardByIdQueryValidator : AbstractValidator<GetRepairCardByIdQuery>
{
    public GetRepairCardByIdQueryValidator()
    {
        RuleFor(x => x.RepairCardId)
            .GreaterThan(0).WithMessage("RepairCardId must be greater than zero.");
    }
}