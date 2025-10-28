using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Suppliers;

public static class SupplierErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Supplier.NameRequired", "Supplier name is required.");

    public static readonly Error PhoneRequired =
        Error.Validation("Supplier.PhoneRequired", "Supplier phone number is required.");

    public static readonly Error NotFound =
        Error.NotFound("Supplier.NotFound", "Supplier not found.");
}