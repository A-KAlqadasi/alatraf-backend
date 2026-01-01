using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Domain.Services.Events;

public sealed record AppointmentIsTomorrow : DomainEvent
{
    public int AppointmentId { get; set; }
}