using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Features.Organization.Rooms.Dtos;

namespace AlatrafClinic.Application.Features.Organization.Sections.Dtos;
public sealed record SectionDto(
    int Id,
    string Name,
    int DepartmentId,
    List<RoomDto>? Rooms = null
);