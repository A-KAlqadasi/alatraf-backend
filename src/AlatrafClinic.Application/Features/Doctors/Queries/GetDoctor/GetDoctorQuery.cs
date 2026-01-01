using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetDoctor;

public sealed record GetDoctorQuery(int DoctorId) : IRequest<Result<DoctorDto>>;