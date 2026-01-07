using FluentValidation;

namespace AlatrafClinic.Application.Features.Identity.Queries.GenerateTokens;

public sealed class GenerateTokenQueryValidator : AbstractValidator<LoginRequest>
{
    public GenerateTokenQueryValidator()
    {
        RuleFor(request => request.UserName)
            .NotNull().NotEmpty()
            .WithErrorCode("UserName_Null_Or_Empty")
            .WithMessage("UserName cannot be null or empty");

        RuleFor(request => request.Password)
            .NotNull().NotEmpty()
            .WithErrorCode("Password_Null_Or_Empty")
            .WithMessage("Password cannot be null or empty.");
    }
}