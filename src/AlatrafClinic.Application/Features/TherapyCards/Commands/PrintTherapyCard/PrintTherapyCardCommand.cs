using AlatrafClinic.Application.Common.Printing.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.PrintTherapyCard;

public record PrintTherapyCardCommand(int TherapyCardId) : IRequest<Result<PdfDto>>;
