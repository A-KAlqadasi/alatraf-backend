using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Features.People.Doctors.Dtos;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Dtos;

public class RoomDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;
    public List<DoctorDto> Doctors { get; set; } = new();
}