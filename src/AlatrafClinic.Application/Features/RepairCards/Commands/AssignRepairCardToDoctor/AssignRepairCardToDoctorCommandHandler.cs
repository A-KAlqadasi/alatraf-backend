
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.AssignRepairCardToDoctor;

public class AssignRepairCardToDoctorCommandHandler : IRequestHandler<AssignRepairCardToDoctorCommand, Result<Updated>>
{
    private readonly ILogger<AssignRepairCardToDoctorCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public AssignRepairCardToDoctorCommandHandler(ILogger<AssignRepairCardToDoctorCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Updated>> Handle(AssignRepairCardToDoctorCommand command, CancellationToken ct)
    {
        var repairCard = await _unitOfWork.RepairCards.GetByIdAsync(command.RepairCardId, ct);
        if (repairCard is null)
        {
            _logger.LogError("Repair card with Id {repairCardId} not found to assign to doctor", command.RepairCardId);
            return RepairCardErrors.RepairCardNotFound;
        }

        // here I will check from section room Id if active
        var doctorSectionRoom = await _unitOfWork.DoctorSectionRooms.GetActiveAssignmentByDoctorAndSectionIdsAsync(command.DoctorId, command.SectionId, ct);
        if (doctorSectionRoom is null)
        {
           _logger.LogError("Section {sectionId} doesn't have active assignement for doctor {doctorId}", command.SectionId, command.DoctorId);

            return DoctorSectionRoomErrors.DoctorSectionRoomNotFound;
        }

        if (!doctorSectionRoom.IsActive)
        {
            _logger.LogError("Doctor {doctorId}, dons't have active assignement in section {sectionId}", command.DoctorId, command.SectionId);

            return DoctorSectionRoomErrors.AssignmentAlreadyEnded;
        }

        var result = repairCard.AssignRepairCardToDoctor(doctorSectionRoom.Id);
        
        if (result.IsError)
        {
            _logger.LogError("Failed to assign repair card with Id {repairCardId} to doctor", command.RepairCardId);

            return result.Errors;
        }

        await _unitOfWork.RepairCards.UpdateAsync(repairCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Repair card {repairCardId} assigned to doctorSectionId {doctorSectionId}", command.RepairCardId, doctorSectionRoom.Id);

        return Result.Updated;
    }
}