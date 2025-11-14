using AlatrafClinic.Application.Features.People.Doctors.Dtos;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Queries.GetTechniciansDropdown;

public sealed record GetTechniciansDropdownQuery : IRequest<List<TechnicianDto>>;