using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistSessions;

public sealed record GetTherapistSessionsQuery(
    int DoctorSectionRoomId,
    DateOnly? Date = null,
    string? PatientName = null,
    int? TherapyCardId = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<PaginatedList<TherapistSessionProgramDto>>>;
