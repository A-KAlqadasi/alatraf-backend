using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.ResetUserPassword;

public sealed class ResetUserPasswordCommandValidator
    : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty();
    }
}
