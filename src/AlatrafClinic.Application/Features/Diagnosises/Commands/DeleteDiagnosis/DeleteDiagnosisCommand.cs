using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Commands.DeleteDiagnosis;

public record DeleteDiagnosisCommand(int diagnosisId) : IRequest<Result<Deleted>>;