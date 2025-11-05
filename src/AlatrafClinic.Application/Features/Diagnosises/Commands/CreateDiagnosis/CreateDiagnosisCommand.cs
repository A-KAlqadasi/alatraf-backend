using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Commands.CreateDiagnosis;

public sealed record CreateDiagnosisCommand(
    int ticketId,
    string diagnosisText,
    DateTime injuryDate,
    List<int> injuryReasons,
    List<int> injurySides,
    List<int> injuryTypes,
    int patientId,
    DiagnosisType diagnosisType
) : IRequest<Result<DiagnosisDto>>;