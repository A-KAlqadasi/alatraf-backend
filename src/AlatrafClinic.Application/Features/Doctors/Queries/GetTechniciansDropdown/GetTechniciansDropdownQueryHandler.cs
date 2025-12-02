using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Mappers;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechniciansDropdown;

public class GetTechniciansDropdownQueryHandler : IRequestHandler<GetTechniciansDropdownQuery, List<TechnicianDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTechniciansDropdownQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<List<TechnicianDto>> Handle(GetTechniciansDropdownQuery query, CancellationToken ct)
    {
        var technicianAssignements = await _unitOfWork.DoctorSectionRooms.GetTechniciansActiveAssignmentsAsync(ct);

        return technicianAssignements.ToTechnicianDtos();
    }
}