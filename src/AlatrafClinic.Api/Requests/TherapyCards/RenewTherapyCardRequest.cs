using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.TherapyCards.Enums;

namespace AlatrafClinic.Api.Requests.TherapyCards;

public class RenewTherapyCardRequest
{
    [Required(ErrorMessage = "TicketId is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "TicketId must be greater than 0.")]
    public int TicketId { get; set; }
    
    [Required(ErrorMessage = "ProgramStartDate is required.")]
    [DataType(DataType.Date)]
    public DateOnly ProgramStartDate { get; set; }    
    public DateOnly? ProgramEndDate { get; set; }
    
    [Required(ErrorMessage = "Number of sessions is required")]
    public int NumberOfSessions { get; set; }

    [Required(ErrorMessage= "TherapyCardType is required.")]
    [EnumDataType(typeof(TherapyCardType), ErrorMessage = "Invalid Therapy Card Type.")]
    public TherapyCardType TherapyCardType { get; set; }

    [MinLength(1, ErrorMessage = "At least one program must be provided.")]
    public List<TherapyCardMedicalProgramRequest> Programs { get; set; } = new();
    
    public string? Notes { get; set; }
}