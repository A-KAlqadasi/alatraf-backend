using AlatrafClinic.Domain.Services.Enums;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Holidays.Commands.CreateHoliday;

public sealed class CreateHolidayCommandValidator : AbstractValidator<CreateHolidayCommand>
{
  public CreateHolidayCommandValidator()
  {
    RuleFor(x => x.Name)
        .NotEmpty()
        .WithMessage("Holiday name is required.");

    RuleFor(x => x.Type)
        .IsInEnum()
        .WithMessage("Invalid holiday type.");

    RuleFor(x => x.StartDate)
        .NotEmpty();

    RuleFor(x => x.EndDate)
        .GreaterThanOrEqualTo(x => x.StartDate)
        .When(x => x.EndDate.HasValue && x.Type == HolidayType.Temporary)
        .WithMessage("End date must be equal or after start date for temporary holidays.");

    RuleFor(x => x.IsRecurring)
        .Must((cmd, isRecurring) =>
            cmd.Type == HolidayType.Fixed ? isRecurring == true : true)
        .WithMessage("Fixed holidays must be recurring.");
  }
}