using AlatrafClinic.Application.Features.Doctors.Dtos;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechniciansDropdown;

public sealed record GetTechniciansDropdownQuery : IRequest<List<TechnicianDto>>;