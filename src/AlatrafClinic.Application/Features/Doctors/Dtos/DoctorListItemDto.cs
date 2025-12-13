namespace AlatrafClinic.Application.Features.Doctors.Dtos;

public sealed class DoctorListItemDto
{
  public int DoctorId { get; set; }
  public string FullName { get; set; } = string.Empty;
  public string? Specialization { get; set; }

  public int DepartmentId { get; set; }
  public string DepartmentName { get; set; } = string.Empty;

  public int? SectionId { get; set; }
  public string? SectionName { get; set; }
  public int? RoomId { get; set; }
  public string? RoomName { get; set; }

  public DateOnly? AssignDate { get; set; }
  public bool IsActiveAssignment { get; set; }
  
}