using System.Data;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Services.Commands.CreateService;

public class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Service name is required.")
            .MaximumLength(200).WithMessage("Service name must not exceed 200 characters.");
        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Department ID must be greater than zero.");
    }
}