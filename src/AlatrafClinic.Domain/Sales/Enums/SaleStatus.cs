namespace AlatrafClinic.Domain.Sales.Enums;

public enum SaleStatus : byte
{
    Draft = 0,

    // Confirmed is the terminal happy-path state for the sale saga
    Confirmed = 1,

    // Alias kept for backward compatibility with legacy handlers
    Posted = Confirmed,

    // Canceled is the failure path after validation/reservation problems
    Canceled = 2,

    // British spelling kept to avoid breaking older code
    Cancelled = Canceled
}