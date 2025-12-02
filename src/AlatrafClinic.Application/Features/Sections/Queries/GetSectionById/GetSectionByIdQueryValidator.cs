using FluentValidation;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetSectionById;

public class GetSectionByIdQueryValidator : AbstractValidator<GetSectionByIdQuery>
{
    public GetSectionByIdQueryValidator()
    {
        RuleFor(x => x.SectionId)
            .GreaterThan(0).WithMessage("Section ID must be greater than zero.");
    }
}