using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Inventory.Suppliers;

public class Supplier : AuditableEntity<int>
{
    public string SupplierName { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;

    private Supplier() { }
    private Supplier(string supplierName, string phone)
    {
        SupplierName = supplierName;
        Phone = phone;
    }
    public static Result<Supplier> Create(string name, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return SupplierErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            return SupplierErrors.PhoneRequired;
        }

        return new Supplier(name, phone);
    }
    public Result<Updated> Update(string name, string phone)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return SupplierErrors.NameRequired;
        }

        if (string.IsNullOrWhiteSpace(phone))
        {
            return SupplierErrors.PhoneRequired;
        }

        SupplierName = name;
        Phone = phone;
        return Result.Updated;
    }
}