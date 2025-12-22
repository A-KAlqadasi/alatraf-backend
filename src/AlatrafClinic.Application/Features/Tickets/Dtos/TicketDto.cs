using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Application.Features.Tickets.Dtos;

public class TicketDto
{
    public int TicketId { get; set; }
    public ServiceDto? Service { get; set; }
    public PatientDto? Patient { get; set; }
    public string TicketStatus { get; set; }  = string.Empty;
}