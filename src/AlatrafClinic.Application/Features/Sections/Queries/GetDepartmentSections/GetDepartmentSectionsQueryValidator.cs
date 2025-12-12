using FluentValidation;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetDepartmentSections;

public class GetDepartmentSectionsQueryValidator : AbstractValidator<GetDepartmentSectionsQuery>
{
    public GetDepartmentSectionsQueryValidator()
    {
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("DepartmentId must be greater than zero.");
    }
}