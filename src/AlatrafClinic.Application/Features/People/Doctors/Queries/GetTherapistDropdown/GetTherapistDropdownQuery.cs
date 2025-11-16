using AlatrafClinic.Application.Features.People.Doctors.Dtos;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Queries.GetTherapistDropdown;

public sealed record GetTherapistDropdownQuery : IRequest<List<TherapistDto>>;