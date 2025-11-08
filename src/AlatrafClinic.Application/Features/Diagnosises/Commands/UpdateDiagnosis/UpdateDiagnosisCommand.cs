using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Commands.UpdateDiagnosis;

public sealed record UpdateDiagnosisCommand(
    int diagnosisId,
    int ticketId,
    string diagnosisText,
    DateTime injuryDate,
    List<int> injuryReasons,
    List<int> injurySides,
    List<int> injuryTypes,
    int patientId,
    DiagnosisType diagnosisType,
    List<(int MedicalProgramId, int duration, string? notes)>? programs = null,
    List<(int partId, int unitId, int quantity, decimal price)>? industrialParts = null,
    List<(int itemId, int unitId, decimal quantity)>? items = null
) : IRequest<Result<Updated>>;