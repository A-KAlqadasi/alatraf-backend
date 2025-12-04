using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.UpdateStore;

public sealed class UpdateStoreCommandValidator : AbstractValidator<UpdateStoreCommand>
{
    public UpdateStoreCommandValidator()
    {
        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Store ID must be greater than zero.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Store name is required.")
            .MaximumLength(200).WithMessage("Store name must not exceed 200 characters.");
    }
}
