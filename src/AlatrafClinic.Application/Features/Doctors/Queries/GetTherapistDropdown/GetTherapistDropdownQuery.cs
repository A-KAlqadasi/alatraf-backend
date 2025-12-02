using AlatrafClinic.Application.Features.Doctors.Dtos;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistDropdown;

public sealed record GetTherapistDropdownQuery : IRequest<List<TherapistDto>>;