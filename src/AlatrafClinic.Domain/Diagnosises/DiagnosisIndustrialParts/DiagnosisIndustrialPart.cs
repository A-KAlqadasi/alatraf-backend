using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

namespace AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;

public class DiagnosisIndustrialPart : AuditableEntity<int>
{
    public int? DiagnosisId { get; set; }
    public Diagnosis? Diagnosis { get; set; }
    public int IndustrialPartUnitId { get; set; }
    public IndustrialPartUnit? IndustrialPartUnit { get; set; }
    public int? DoctorSectionRoomId { get; set; }
    public DoctorSectionRoom? DoctorSectionRoom { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public DateTime DoctorAssignDate { get; set; }
    public int? RepairCardId { get; set; }
    public RepairCard? RepairCard { get; set; }

    private DiagnosisIndustrialPart() { }

    private DiagnosisIndustrialPart(int industrialPartUnitId, int quantity, decimal price)
    {
        IndustrialPartUnitId = industrialPartUnitId;
        Quantity = quantity;
        Price = price;
    }

    public static Result<DiagnosisIndustrialPart> Create(int industrialPartUnitId, int quantity, decimal price)
    {
        if (industrialPartUnitId <= 0)
        {
            return DiagnosisIndustrialPartErrors.IndustrialPartUnitIdInvalid;
        }
        if (quantity <= 0)
        {
            return DiagnosisIndustrialPartErrors.QuantityInvalid;
        }
        if (price <= 0)
        {
            return DiagnosisIndustrialPartErrors.PriceInvalid;
        }
        return new DiagnosisIndustrialPart(industrialPartUnitId, quantity, price);
    }
    public Result<Updated> Update(int industrialPartUnitId, int quantity, decimal price)
    {
        if (industrialPartUnitId <= 0)
        {
            return DiagnosisIndustrialPartErrors.IndustrialPartUnitIdInvalid;
        }
        if (quantity <= 0)
        {
            return DiagnosisIndustrialPartErrors.QuantityInvalid;
        }
        if (price <= 0)
        {
            return DiagnosisIndustrialPartErrors.PriceInvalid;
        }
        IndustrialPartUnitId = industrialPartUnitId;
        Quantity = quantity;
        Price = price;

        return Result.Updated;
    }
    public Result<Updated> AssignDoctor(int doctorSectionRoomId)
    {
        if (doctorSectionRoomId <= 0)
        {
            return DiagnosisIndustrialPartErrors.DoctorSectionRoomIdInvalid;
        }
        
        DoctorSectionRoomId = doctorSectionRoomId;
        DoctorAssignDate = DateTime.UtcNow;
        return Result.Updated;
    }
}