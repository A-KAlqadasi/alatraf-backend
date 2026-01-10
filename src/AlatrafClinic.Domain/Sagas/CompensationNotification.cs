// Domain/Sagas/CompensationNotification.cs
namespace AlatrafClinic.Domain.Sagas
{
    public class CompensationNotification
    {
        public int Id { get; set; }
        public Guid SagaId { get; set; }
        public bool Success { get; set; }
        public DateTime SentAt { get; set; }
    }
}
