using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

namespace AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;

public class DiagnosisIndustrialPart : AuditableEntity<int>
{
    public int DiagnosisId { get; private set; }
    public Diagnosis? Diagnosis { get; set; }
    public int IndustrialPartUnitId { get; private set; }
    public IndustrialPartUnit? IndustrialPartUnit { get; private set; }
    public int? DoctorSectionRoomId { get; private set; }
    public DoctorSectionRoom? DoctorSectionRoom { get; set; }
    public int Quantity { get; private set; }
    public decimal Price { get; private set; }
    public DateTime? DoctorAssignDate { get; private set; }

    private DiagnosisIndustrialPart() { }

    private DiagnosisIndustrialPart(int diagnosisId, int industrialPartUnitId, int quantity, decimal price)
    {
        DiagnosisId = diagnosisId;
        IndustrialPartUnitId = industrialPartUnitId;
        Quantity = quantity;
        Price = price;
    }

    public static Result<DiagnosisIndustrialPart> Create(int diagnosisId, int industrialPartUnitId, int quantity, decimal price)
    {
        if(diagnosisId <= 0)
        {
            return DiagnosisIndustrialPartErrors.DiagnosisIdInvalid;
        }
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
        return new DiagnosisIndustrialPart(diagnosisId, industrialPartUnitId, quantity, price);
    }
    public Result<Updated> Update(int diagnosisId, int industrialPartUnitId, int quantity, decimal price)
    {
        if (diagnosisId <= 0)
        {
            return DiagnosisIndustrialPartErrors.DiagnosisIdInvalid;
        }
        
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