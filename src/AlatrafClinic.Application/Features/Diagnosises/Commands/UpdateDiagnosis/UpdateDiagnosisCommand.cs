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
    DiagnosisType diagnosisType
) : IRequest<Result<Updated>>;