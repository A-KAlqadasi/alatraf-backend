using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;

public static class DiagnosisIndustrialPartErrors
{
    public static readonly Error IndustrialPartUnitIdInvalid= Error.Validation("DiagnosisIndustrialPart.IndustrialPartUnitId","رقم وحدة الطرف الصناعي غير صالح");
    public static readonly Error QuantityInvalid = Error.Validation("DiagnosisIndustrialPart.Quantity", "الكمية غير صالحة");
    public static readonly Error PriceInvalid = Error.Validation("DiagnosisIndustrialPart.Price", "السعر غير صالح");
    public static readonly Error DoctorSectionRoomIdInvalid = Error.Validation("DiagnosisIndustrialPart.DoctorSectionRoomId", "رقم قسم الطبيب غير صالح");
    public static readonly Error DiagnosisIdInvalid = Error.Validation("DiagnosisIndustrialPart.DiagnosisId", "رقم التشخيص غير صالح");
    public static readonly Error DiagnosisIndustrialPartNotFound = Error.NotFound("DiagnosisIndustrialPart.NotFound", "الطرف الصناعي للتشخيص غير موجود");
    public static readonly Error DiagnosisIndustrialPartAlreadyAssignedToDoctor = Error.Conflict("DiagnosisIndustrialPart.AlreadyAssignedToDoctor", "الطرف الصناعي للتشخيص معين بالفعل لطبيب");
}