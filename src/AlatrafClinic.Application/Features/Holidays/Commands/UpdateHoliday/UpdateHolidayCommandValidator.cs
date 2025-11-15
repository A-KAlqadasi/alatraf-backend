using AlatrafClinic.Domain.Services.Enums;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Holidays.Commands.UpdateHoliday;

public sealed class UpdateHolidayCommandValidator : AbstractValidator<UpdateHolidayCommand>
{
  public UpdateHolidayCommandValidator()
  {
    RuleFor(x => x.HolidayId)
        .GreaterThan(0)
        .WithMessage("HolidayId is required.");

    RuleFor(x => x.Name)
        .NotEmpty()
        .WithMessage("Holiday name is required.");

    RuleFor(x => x.Type)
        .IsInEnum()
        .WithMessage("Invalid holiday type.");

    RuleFor(x => x.StartDate)
        .NotEmpty()
        .WithMessage("StartDate is required.");

    RuleFor(x => x.EndDate)
        .GreaterThanOrEqualTo(x => x.StartDate)
        .When(x => x.EndDate.HasValue && x.Type == HolidayType.Temporary)
        .WithMessage("End date must be equal or after StartDate for temporary holidays.");

    RuleFor(x => x.IsRecurring)
        .Must((cmd, isRecurring) =>
            cmd.Type == HolidayType.Fixed ? isRecurring == true : true)
        .WithMessage("Fixed holidays must be recurring.");
  }
}