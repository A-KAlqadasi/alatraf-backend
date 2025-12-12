using FluentValidation;

namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRoomsBySectionId;

public class GetRoomsBySectionIdQueryValidator : AbstractValidator<GetRoomsBySectionIdQuery>
{
    public GetRoomsBySectionIdQueryValidator()
    {
        RuleFor(x => x.SectionId)
            .GreaterThan(0).WithMessage("SectionId must be greater than zero.");
    }
}