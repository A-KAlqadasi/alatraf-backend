using System.Text.Json.Serialization;

using AlatrafClinic.Domain.Common;

public sealed record SaleCreatedDomainEvent : DomainEvent
{
    [JsonInclude]
    public int SaleId { get; private set; }
    [JsonInclude]
    public int DiagnosisId { get; private set; }

    private SaleCreatedDomainEvent() { } // مهم

    public SaleCreatedDomainEvent(int saleId, int diagnosisId)
    {
        SaleId = saleId;
        DiagnosisId = diagnosisId;
    }
}

