using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianAssignedIndustrialParts;

public class GetTechnicianAssignedIndustrialPartsQueryHandler : IRequestHandler<GetTechnicianAssignedIndustrialPartsQuery, Result<TechnicianIndustrialPartsResultDto>>
{
    private readonly IAppDbContext _context;

    public GetTechnicianAssignedIndustrialPartsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<TechnicianIndustrialPartsResultDto>> Handle(GetTechnicianAssignedIndustrialPartsQuery query, CancellationToken ct)
    {
        var today = AlatrafClinicConstants.TodayDate;

        var result = await _context.DoctorSectionRooms
            .AsNoTracking()
            .Where(a => a.Id == query.DoctorSectionRoomId && a.IsActive)
            .Select(a => new TechnicianIndustrialPartsResultDto
            {
                DoctorSectionRoomId = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.Person!.FullName,
                SectionId = a.SectionId,
                SectionName = a.Section.Name,
                Date = today
            })
            .FirstOrDefaultAsync(ct);

        if (result is null)
        {
            return Error.NotFound("DoctorSectionRoom.NotFound", "Doctor section room not found or inactive.");
        }

        result.Items = await _context.DiagnosisIndustrialParts
            .AsNoTracking()
            .Where(dip =>
                dip.DoctorSectionRoomId == query.DoctorSectionRoomId &&
                dip.DoctorAssignDate == today)
            .Select(dip => new TechnicianIndustrialPartDto
            {
                DiagnosisIndustrialPartId = dip.Id,
                IndustrialPartUnitId = dip.IndustrialPartUnitId,
                Quantity = dip.Quantity,
                IndustrialPartName = dip.IndustrialPartUnit.IndustrialPart.Name,
                RepairCardId = dip.RepairCardId,
                PatientName = dip.RepairCard!.Diagnosis.Patient.Person.FullName,
                PatientPhoneNumber = dip.RepairCard.Diagnosis.Patient.Person.Phone
            })
            .ToListAsync(ct);

        return result;
    }
}