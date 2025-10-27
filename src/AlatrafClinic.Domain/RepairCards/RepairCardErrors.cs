using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Domain.RepairCards;

public static class RepairCardErrors
{
    public static readonly Error DiagnosisIndustrialPartsAreRequired = Error.Validation("RepairCard.DiagnosisIndustrialParts", "Diagnosis industrial parts are required.");
    public static readonly Error DiagnosisIndustrialPartNotFound = Error.Validation("RepairCard.DiagnosisIndustrialPart", "Diagnosis industrial part not found.");
    public static readonly Error Readonly = Error.Conflict("RepairCard.Readonly", "Repair card is readonly");
    public static Error InvalidStateTransition(RepairCardStatus current, RepairCardStatus next) => Error.Conflict(
       code: "RepairCard.InvalidStateTransition",
       description: $"Repair card Invalid State transition from '{current}' to '{next}'.");
    public static readonly Error AttendanceTimeIsRequired = Error.Validation("RepairCard.AttendanceTime", "Attendance time is required.");

}