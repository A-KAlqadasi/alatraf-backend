using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.DeleteSupplierCommand;

public sealed class DeleteSupplierCommandValidator : AbstractValidator<DeleteSupplierCommand>
{
    public DeleteSupplierCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Supplier ID must be greater than 0.");
    }
}
