using FluentValidation;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetDoctorsBySectionRoom;

public class GetDoctorsBySectionRoomQueryValidator : AbstractValidator<GetDoctorsBySectionRoomQuery>
{
    public GetDoctorsBySectionRoomQueryValidator()
    {
        RuleFor(x => x.SectionId)
            .GreaterThan(0).WithMessage("SectionId must be greater than zero.");

        When(x => x.RoomId.HasValue, () =>
        {
            RuleFor(x => x.RoomId!.Value)
                .GreaterThan(0).WithMessage("RoomId must be greater than zero when provided.");
        });
    }
}