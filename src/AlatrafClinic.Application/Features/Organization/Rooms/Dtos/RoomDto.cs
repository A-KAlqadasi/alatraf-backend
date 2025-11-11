using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Dtos;

public sealed record RoomDto(
    int Id,
    int Number,
    int SectionId
);