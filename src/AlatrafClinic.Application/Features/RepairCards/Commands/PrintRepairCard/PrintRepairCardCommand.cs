using AlatrafClinic.Application.Common.Printing.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.PrintRepairCard;

public record PrintRepairCardCommand(int RepairCardId) : IRequest<Result<PdfDto>>;
