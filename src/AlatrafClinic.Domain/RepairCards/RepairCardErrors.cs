using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Domain.RepairCards;

public static class RepairCardErrors
{
    public static readonly Error DiagnosisIndustrialPartNotFound = Error.Validation("RepairCard.DiagnosisIndustrialPart", "Diagnosis industrial part not found.");
    public static readonly Error Readonly = Error.Conflict("RepairCard.Readonly", "Repair card is readonly");
    public static Error InvalidStateTransition(RepairCardStatus current, RepairCardStatus next) => Error.Conflict(
       code: "RepairCard.InvalidStateTransition",
       description: $"Repair card Invalid State transition from '{current}' to '{next}'.");
    public static readonly Error OrderAlreadyExists = Error.Conflict("RepairCard.OrderAlreadyExists", "Order already exists in the repair card.");

    public static readonly Error InvalidDiagnosisId =
        Error.Validation("Entity.InvalidDiagnosisId", "Invalid diagnosis reference.");
    public static readonly Error InvalidOrder = Error.Validation("RepairCard.InvalidOrder", "Invalid order reference.");
    public static readonly Error ExitCardAlreadyAssigned = Error.Validation("RepairCard.ExitCardAlreadyAssigned", "Exit card is already assigned to this repair card.");
    public static readonly Error RepairCardNotFound = Error.NotFound("RepairCard.NotFound", "Repair card not found.");
    public static readonly Error InvalidStatus = Error.Validation("RepairCard.InvalidStatus", "Invalid status.");
    public static readonly Error PaymentNotFound = Error.NotFound("RepairCard.PaymentNotFound", "Payment for the repair card not found.");
    public static readonly Error NoRepairCardsForPaitent = Error.NotFound("RepairCard.NoRepairCardsForPaitent", "No repair cards found for the patient.");

}