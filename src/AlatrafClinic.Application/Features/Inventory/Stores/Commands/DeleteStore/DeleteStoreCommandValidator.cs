using FluentValidation;

namespace AlatrafClinic.Application.Features.Inventory.Stores.Commands.DeleteStore;

public sealed class DeleteStoreCommandValidator : AbstractValidator<DeleteStoreCommand>
{
    public DeleteStoreCommandValidator()
    {
        RuleFor(x => x.StoreId)
            .GreaterThan(0).WithMessage("Store ID must be greater than zero.");
    }
}
