using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistTodaySessions;

public sealed class GetTherapistTodaySessionsQueryHandler
    : IRequestHandler<GetTherapistTodaySessionsQuery, Result<TherapistTodaySessionsResultDto>>
{
    private readonly IAppDbContext _context;

    public GetTherapistTodaySessionsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TherapistTodaySessionsResultDto>> Handle(
        GetTherapistTodaySessionsQuery query,
        CancellationToken ct)
    {
        var today = AlatrafClinicConstants.TodayDate;

        var header = await _context.DoctorSectionRooms
            .AsNoTracking()
            .Where(a => a.Id == query.DoctorSectionRoomId && a.IsActive)
            .Select(a => new TherapistTodaySessionsResultDto
            {
                DoctorSectionRoomId = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor!.Person!.FullName,

                SectionId = a.SectionId,
                SectionName = a.Section!.Name,

                RoomId = a.RoomId ?? 0,
                RoomName = a.Room!.Name,

                Date = today
            })
            .FirstOrDefaultAsync(ct);

        if (header is null)
        {
            // Replace with your actual error pattern (NotFound, etc.)
            return Error.NotFound(
                "Therapist.AssignmentNotFound",
                "Active therapist assignment not found.");
        }

        header.Items = await _context.SessionPrograms
            .AsNoTracking()
            .Where(sp =>
                sp.DoctorSectionRoomId == query.DoctorSectionRoomId &&
                sp.Session!.SessionDate >= today)
            .OrderByDescending(sp => sp.CreatedAtUtc)
            .Select(sp => new TherapistSessionProgramDto
            {
                SessionProgramId = sp.Id,
                DiagnosisProgramId = sp.DiagnosisProgramId,
                ProgramName = sp.DiagnosisProgram.MedicalProgram!.Name, // adjust if different
                SessionId = sp.SessionId,
                TherapyCardId = sp.Session!.TherapyCardId,
                PatientName = sp.Session!.TherapyCard!.Diagnosis.Patient!.Person!.FullName,
                PatientPhoneNumber = sp.Session!.TherapyCard!.Diagnosis.Patient!.Person.Phone
            })
            .ToListAsync(ct);

        return header;
    }
}