using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.RepairCards.Enums;

namespace AlatrafClinic.Api.Requests.RepairCards;

public sealed class ChangeRepairCardStatusRequest
{
    [Required]
    public RepairCardStatus CardStatus { get; set; }
}