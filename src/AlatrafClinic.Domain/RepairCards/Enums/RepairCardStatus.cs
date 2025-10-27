namespace AlatrafClinic.Domain.RepairCards.Enums;

public enum RepairCardStatus : byte
{
    New = 0,
    AssignedToTechnician,
    InProgress,
    InTraining,
    Completed,
    LegalExit,
    ExitForPractice,
    IllegalExit
}