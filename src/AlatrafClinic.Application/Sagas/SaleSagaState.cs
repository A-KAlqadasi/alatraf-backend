using System;

namespace AlatrafClinic.Application.Sagas;

public sealed class SaleSagaState
{
    public Guid SagaId { get; init; }
    public int TicketId { get; init; }
    public int DiagnosisId { get; init; }
    public int? SaleId { get; set; }

    public bool ValidateStockCompleted { get; set; }
    public bool SaleDraftCreated { get; set; }
    public bool InventoryReserved { get; set; }
    public bool SaleConfirmed { get; set; }
    public bool PaymentCreated { get; set; }

    public decimal TotalAmount { get; set; }
}
