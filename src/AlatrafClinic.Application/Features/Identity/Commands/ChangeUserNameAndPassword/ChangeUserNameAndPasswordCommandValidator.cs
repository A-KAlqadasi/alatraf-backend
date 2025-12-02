using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Commands.ChangeUserNameAndPassword;

public class ChangeUserNameAndPasswordCommandValidator : AbstractValidator<ChangeUserNameAndPasswordCommand>
{
    public ChangeUserNameAndPasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(4).WithMessage("Username must be at least 4 characters long.")
            .MaximumLength(12).WithMessage("Username must not exceed 12 characters.");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

         RuleFor(x=> x.NewPassword)
        .NotEmpty()
        .MinimumLength(6)
        .MaximumLength(20)
        .Matches(@"^[a-zA-Z0-9]+$")
        .WithMessage("Password must be alphanumeric and between 6 to 20 characters long.");
    }
}