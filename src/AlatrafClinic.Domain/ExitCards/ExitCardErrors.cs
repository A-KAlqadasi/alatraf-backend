using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.ExitCards;

public static class ExitCardErrors
{
    public static readonly Error PatientIdIsRequired = Error.Validation("ExitCard.PatientIdIsRequired", "Patient Id is required.");
    public static readonly Error RepairCardIsRequired = Error.Validation("ExitCard.RepairCardIsRequired", "Repair Card is required.");
    public static readonly Error SaleIsRequired = Error.Validation("ExitCard.SaleIsRequired", "Sale is required.");
    public static readonly Error AlreadyAssignedToSale = Error.Validation("ExitCard.AlreadyAssignedToSale", "Exit Card is already assigned to a Sale.");
    public static readonly Error AlreadyAssignedToRepairCard = Error.Validation("ExitCard.AlreadyAssignedToRepairCard", "Exit Card is already assigned to a Repair Card.");
    public static readonly Error ReadOnly = Error.Validation("ExitCard.ReadOnly", "Exit Card is read-only and cannot be modified.");
}