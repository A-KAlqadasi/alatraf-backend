using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.ChangeUserCredentials;

public sealed class ChangeUserCredentialsCommandValidator
    : AbstractValidator<ChangeUserCredentialsCommand>
{
    public ChangeUserCredentialsCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OldPassword).NotEmpty();

        RuleFor(x => x)
            .Must(x => x.NewPassword != null || x.NewUsername != null)
            .WithMessage("At least one credential must be changed.");
    }
}
