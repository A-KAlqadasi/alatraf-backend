using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;

using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.UpdateSupplierCommand;

public sealed class UpdateSupplierCommandValidator : AbstractValidator<UpdateSupplierCommand>
{
    public UpdateSupplierCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Supplier ID must be greater than 0.");

        RuleFor(x => x.SupplierName)
            .NotEmpty()
            .WithMessage("Supplier name is required.")
            .MaximumLength(100)
            .WithMessage("Supplier name must not exceed 100 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Phone number is required.")
            .MaximumLength(20)
            .WithMessage("Phone number must not exceed 20 characters.")
            .Matches(@"^[0-9+\-\s]+$")
            .WithMessage("Phone number can only contain digits, +, -, or spaces.");
    }
}
