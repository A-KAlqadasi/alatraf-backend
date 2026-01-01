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
        
    }
}