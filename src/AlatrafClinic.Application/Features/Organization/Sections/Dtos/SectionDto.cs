using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Features.Organization.Rooms.Dtos;
using AlatrafClinic.Application.Features.People.Doctors.Dtos;

namespace AlatrafClinic.Application.Features.Organization.Sections.Dtos;
public class SectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; } = string.Empty;
    public List<RoomDto>? Rooms { get; set; }
    public List<DoctorDto> Doctors { get; set; } = new();
}