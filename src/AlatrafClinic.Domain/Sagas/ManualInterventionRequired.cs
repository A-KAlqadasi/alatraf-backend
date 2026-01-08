// Domain/Sagas/ManualInterventionRequired.cs
// Domain/Sagas/ManualInterventionRequired.cs
namespace AlatrafClinic.Domain.Sagas
{
    public class ManualInterventionRequired
    {
        public int Id { get; set; }
        public Guid SagaId { get; set; }
        public DateTime RequiredAt { get; set; }
        public string Status { get; set; } = default!;
        public string? ResolutionNotes { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}