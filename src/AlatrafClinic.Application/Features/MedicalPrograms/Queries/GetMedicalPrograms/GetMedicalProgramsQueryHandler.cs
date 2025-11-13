using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Application.Features.MedicalPrograms.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalPrograms;

public class GetMedicalProgramsQueryHandler : IRequestHandler<GetMedicalProgramsQuery, Result<List<MedicalProgramDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetMedicalProgramsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<List<MedicalProgramDto>>> Handle(GetMedicalProgramsQuery query, CancellationToken ct)
    {
        var medicalPrograms = await _unitOfWork.MedicalPrograms.GetAllAsync(ct);
        if (medicalPrograms is null || !medicalPrograms.Any())
        {
           return Error.NotFound("Medical programs not found.");
        }
        return medicalPrograms.ToDtos();
    }
}