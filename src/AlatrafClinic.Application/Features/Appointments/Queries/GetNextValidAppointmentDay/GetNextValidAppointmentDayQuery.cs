using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;

public sealed record GetNextValidAppointmentDayQuery(
    DateOnly? AfterDate = null
) : IRequest<Result<NextAppointmentDayDto>>;
