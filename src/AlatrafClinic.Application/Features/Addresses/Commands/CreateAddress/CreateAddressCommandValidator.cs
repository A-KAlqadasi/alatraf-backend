using FluentValidation;

namespace AlatrafClinic.Application.Features.Addresses.Commands.CreateAddress;

public class CreateAddressCommandValidator :
    AbstractValidator<CreateAddressCommand>
{
    public CreateAddressCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Address Id must be greater than zero.");
            
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Address name is required.")
            .MaximumLength(200).WithMessage("Address name must not exceed 200 characters.");
    }
}