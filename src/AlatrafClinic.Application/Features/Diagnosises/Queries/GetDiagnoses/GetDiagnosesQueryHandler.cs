using Microsoft.EntityFrameworkCore;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnoses;

public class GetDiagnosesQueryHandler
    : IRequestHandler<GetDiagnosesQuery, Result<PaginatedList<DiagnosisDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetDiagnosesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<DiagnosisDto>>> Handle(GetDiagnosesQuery query, CancellationToken ct)
    {
        var specification = new DiagnosesFilter(query);

        var totalCount = await _unitOfWork.Diagnoses.CountAsync(specification, ct);

        var diagnoses = await _unitOfWork.Diagnoses
            .ListAsync(specification, specification.Page, specification.PageSize, ct);

        var items = diagnoses
            .Select(d => new DiagnosisDto
            {
                DiagnosisId   = d.Id,
                DiagnosisText = d.DiagnosisText,
                InjuryDate    = d.InjuryDate,

                TicketId      = d.TicketId,
                PatientId     = d.PatientId,
                PatientName   = d.Patient != null && d.Patient.Person != null
                                    ? d.Patient.Person.FullName
                                    : string.Empty,

                DiagnosisType = d.DiagnoType.ToArabicDiagnosisType(),

                InjuryReasons = d.InjuryReasons.ToDtos(),
                InjurySides   = d.InjurySides.ToDtos(),
                InjuryTypes   = d.InjuryTypes.ToDtos(),

                HasRepairCard   = d.RepairCard != null,
                HasSale         = d.Sale != null,
                HasTherapyCards = d.TherapyCard != null,
            })
            .ToList();

        var result = new PaginatedList<DiagnosisDto>
        {
            Items      = items,
            PageNumber = specification.Page,
            PageSize   = specification.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)specification.PageSize)
        };

        return result;
    }
}