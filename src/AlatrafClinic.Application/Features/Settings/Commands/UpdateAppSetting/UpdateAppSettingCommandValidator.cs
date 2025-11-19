using FluentValidation;

namespace AlatrafClinic.Application.Features.Settings.Commands.UpdateAppSetting;

public sealed class UpdateAppSettingCommandValidator : AbstractValidator<UpdateAppSettingCommand>
{
  public UpdateAppSettingCommandValidator()
  {
    RuleFor(x => x.Key)
        .NotEmpty()
        .MaximumLength(200);

    RuleFor(x => x.Value)
        .NotNull();
  }
}