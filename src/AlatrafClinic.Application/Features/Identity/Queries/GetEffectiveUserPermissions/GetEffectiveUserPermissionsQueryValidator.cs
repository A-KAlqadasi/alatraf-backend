using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Queries.GetEffectiveUserPermissions;

public sealed class GetEffectiveUserPermissionsQueryValidator
    : AbstractValidator<GetEffectiveUserPermissionsQuery>
{
    public GetEffectiveUserPermissionsQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}

