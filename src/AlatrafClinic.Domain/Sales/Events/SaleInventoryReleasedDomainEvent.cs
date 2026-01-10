using AlatrafClinic.Domain.Common;


namespace AlatrafClinic.Domain.Sales
{
    public sealed record SaleInventoryReleasedDomainEvent : DomainEvent
    {
        public int SaleId { get; }

        public SaleInventoryReleasedDomainEvent(int saleId)
        {
            SaleId = saleId;
        }
    }
}