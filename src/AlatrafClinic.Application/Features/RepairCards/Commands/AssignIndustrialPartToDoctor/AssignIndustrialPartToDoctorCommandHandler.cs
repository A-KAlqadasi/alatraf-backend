
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.AssignIndustrialPartToDoctor;

public class AssignIndustrialPartToDoctorCommandHandler : IRequestHandler<AssignIndustrialPartToDoctorCommand, Result<Updated>>
{
    private readonly ILogger<AssignIndustrialPartToDoctorCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AssignIndustrialPartToDoctorCommandHandler(ILogger<AssignIndustrialPartToDoctorCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Updated>> Handle(AssignIndustrialPartToDoctorCommand command, CancellationToken ct)
    {
        var repairCard = await _unitOfWork.RepairCards.GetByIdAsync(command.RepairCardId, ct);
        if (repairCard is null)
        {
            _logger.LogError("Repair card with id {RepairCardId} not found", command.RepairCardId);
            return RepairCardErrors.RepairCardNotFound;
        }

        foreach (var doctorPart in command.DoctorIndustrialParts)
        {
            var industrialPart = await _unitOfWork.Diagnoses.GetDiagnosisIndustrialPartByIdAsync(doctorPart.DiagnosisIndustrialPartId, ct);

            if (industrialPart is null)
            {
                _logger.LogError("Industrial part with id {IndustrialPartId} not found", doctorPart.DiagnosisIndustrialPartId);

                return DiagnosisIndustrialPartErrors.DiagnosisIndustrialPartNotFound;
            }

            if (industrialPart.DoctorSectionRoom is not null)
            {
                _logger.LogError("Industrial part with id {IndustrialPartId} is already assigned to a doctor", doctorPart.DiagnosisIndustrialPartId);

                return DiagnosisIndustrialPartErrors.DiagnosisIndustrialPartAlreadyAssignedToDoctor;
            }
            
            var doctorSectionRoom = await _unitOfWork.DoctorSectionRooms.GetActiveAssignmentByDoctorAndSectionIdsAsync(doctorPart.DoctorId, doctorPart.SectionId, ct);

            if (doctorSectionRoom is null)
            {
                _logger.LogError("Section {sectionId} doesn't have active assignement for doctor {doctorId}", doctorPart.SectionId, doctorPart.DoctorId);

                return DoctorSectionRoomErrors.DoctorSectionRoomNotFound;
            }

            if (!doctorSectionRoom.IsActive)
            {
                _logger.LogError("Doctor {doctorId}, dons't have active assignement in section {sectionId}", doctorPart.DoctorId, doctorPart.SectionId);

                return DoctorSectionRoomErrors.AssignmentAlreadyEnded;
            }

            repairCard.AssignSpecificIndustrialPartToDoctor(doctorPart.DiagnosisIndustrialPartId, doctorSectionRoom.Id);
        }
        
        await _unitOfWork.RepairCards.UpdateAsync(repairCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        _logger.LogInformation("Assigned industrial parts to doctors for repair card with id {RepairCardId}", command.RepairCardId);
        
        return Result.Updated;
    }
}