using FluentValidation;

namespace AlatrafClinic.Application.Features.Settings.Commands;

public sealed class CreateAppSettingCommandValidator : AbstractValidator<CreateAppSettingCommand>
{
  public CreateAppSettingCommandValidator()
  {
    RuleFor(x => x.Key)
        .NotEmpty()
        .MaximumLength(200);

    RuleFor(x => x.Value)
        .NotNull();
  }
}