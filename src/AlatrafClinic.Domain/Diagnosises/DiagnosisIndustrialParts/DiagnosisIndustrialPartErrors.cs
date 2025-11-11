using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;

public static class DiagnosisIndustrialPartErrors
{
    public static readonly Error IndustrialPartUnitIdInvalid= Error.Validation("DiagnosisIndustrialPart.IndustrialPartUnitId","IndustrialPartUnitId is invalid");
    public static readonly Error QuantityInvalid = Error.Validation("DiagnosisIndustrialPart.Quantity", "Quantity is invalid");
    public static readonly Error PriceInvalid = Error.Validation("DiagnosisIndustrialPart.Price", "Price is invalid");
    public static readonly Error DoctorSectionRoomIdInvalid = Error.Validation("DiagnosisIndustrialPart.DoctorSectionRoomId", "DoctorSectionRoomId is invalid");
    public static readonly Error DiagnosisIdInvalid = Error.Validation("DiagnosisIndustrialPart.DiagnosisId", "DiagnosisId is invalid");
    public static readonly Error DiagnosisIndustrialPartNotFound = Error.NotFound("DiagnosisIndustrialPart.NotFound", "Diagnosis industrial part not found");
    public static readonly Error DiagnosisIndustrialPartAlreadyAssignedToDoctor = Error.Conflict("DiagnosisIndustrialPart.AlreadyAssignedToDoctor", "Diagnosis industrial part is already assigned to a doctor");
}