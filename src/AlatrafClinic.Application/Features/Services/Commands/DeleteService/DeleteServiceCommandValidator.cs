using FluentValidation;

namespace AlatrafClinic.Application.Features.Services.Commands.DeleteService;

public class DeleteServiceCommandValidator : AbstractValidator<DeleteServiceCommand>
{
    public DeleteServiceCommandValidator()
    {
        RuleFor(x => x.ServiceId)
            .GreaterThan(0).WithMessage("ServiceId must be greater than zero.");
    }
}