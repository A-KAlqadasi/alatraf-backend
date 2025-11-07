using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.CreateSupplierCommand;

public sealed class CreateSupplierValidator : AbstractValidator<CreateSupplierCommand>
{
    public CreateSupplierValidator()
    {
        RuleFor(x => x.SupplierName)
            .NotEmpty().WithMessage("Supplier name is required.")
            .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters.");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.")
            .Matches(@"^[0-9+\-\s]+$").WithMessage("Phone number format is invalid.");
    }
}
