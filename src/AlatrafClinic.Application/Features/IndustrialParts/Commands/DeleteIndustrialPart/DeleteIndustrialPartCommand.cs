using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.DeleteIndustrialPart;

public sealed record DeleteIndustrialPartCommand(int IndustrialPartId) : IRequest<Result<Deleted>>;