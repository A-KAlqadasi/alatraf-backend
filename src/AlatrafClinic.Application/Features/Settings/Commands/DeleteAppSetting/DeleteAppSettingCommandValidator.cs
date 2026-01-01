using FluentValidation;

namespace AlatrafClinic.Application.Features.Settings.Commands.DeleteAppSetting;

public sealed class DeleteAppSettingCommandValidator : AbstractValidator<DeleteAppSettingCommand>
{
  public DeleteAppSettingCommandValidator()
  {
    RuleFor(x => x.Key)
        .NotEmpty()
        .MaximumLength(200);
  }
}