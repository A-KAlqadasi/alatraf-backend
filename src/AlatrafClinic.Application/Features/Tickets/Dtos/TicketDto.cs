using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Application.Features.Tickets.Dtos;

public class TicketDto
{
    public int Id { get; set; }
    public ServiceDto? Service { get; set; }
    public PatientDto? Patient { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.New;
}