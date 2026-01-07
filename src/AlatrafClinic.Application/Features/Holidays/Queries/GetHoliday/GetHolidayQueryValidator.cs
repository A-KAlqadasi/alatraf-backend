using FluentValidation;

namespace AlatrafClinic.Application.Features.Holidays.Queries.GetHoliday;

public class GetHolidayQueryValidator : AbstractValidator<GetHolidayQuery>
{
    public GetHolidayQueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Holiday Id must be greater than 0.");
    }
}