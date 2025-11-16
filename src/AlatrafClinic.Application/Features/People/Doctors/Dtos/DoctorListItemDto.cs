namespace AlatrafClinic.Application.Features.People.Doctors.Dtos;

public sealed class DoctorListItemDto
{
  public int DoctorId { get; set; }
  public string FullName { get; set; } = string.Empty;
  public string? Specialization { get; set; }

  public int DepartmentId { get; set; }
  public string DepartmentName { get; set; } = string.Empty;

  public int? CurrentSectionId { get; set; }
  public string? CurrentSectionName { get; set; }
  public int? CurrentRoomId { get; set; }
  public string? CurrentRoomName { get; set; }

  public DateTime? AssignDate { get; set; }
  public bool IsActiveAssignment { get; set; }
  
}