namespace AlatrafClinic.Application.Sagas;

public enum SaleSagaStep
{
    ValidateStock = 1,
    CreateSaleDraft = 2,
    ReserveInventory = 3,
    ConfirmSale = 4,
    CreatePayment = 5
}
