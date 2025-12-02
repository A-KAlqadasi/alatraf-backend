using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Mappers;

using MediatR;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistDropdown;

public class GetTherapistDropdownQueryHandler : IRequestHandler<GetTherapistDropdownQuery, List<TherapistDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTherapistDropdownQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<List<TherapistDto>> Handle(GetTherapistDropdownQuery query, CancellationToken ct)
    {
        var therapistAssignements = await _unitOfWork.DoctorSectionRooms.GetTherapistsActiveAssignmentsAsync(ct);

        return therapistAssignements.ToTherapistDtos();
    }
}