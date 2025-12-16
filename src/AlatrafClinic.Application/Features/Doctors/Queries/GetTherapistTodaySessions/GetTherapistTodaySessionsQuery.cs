using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistTodaySessions;

public sealed record GetTherapistTodaySessionsQuery(
    int DoctorSectionRoomId
) : IRequest<Result<TherapistTodaySessionsResultDto>>;
