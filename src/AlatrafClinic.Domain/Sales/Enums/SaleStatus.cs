namespace AlatrafClinic.Domain.Sales.Enums;

public enum SaleStatus : byte
{
    Draft = 0,     // doctor added items; editable; no stock impact
    Posted = 1,    // payment confirmed; exchange order created; stock decreased; locked
    Cancelled = 2  // aborted before posting; no stock impact
}