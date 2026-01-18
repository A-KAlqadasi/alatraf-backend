using AlatrafClinic.Application.Common.Printing.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.PrintSession;

public sealed record PrintSessionCommand(int SessionId) : IRequest<Result<PdfDto>>;
