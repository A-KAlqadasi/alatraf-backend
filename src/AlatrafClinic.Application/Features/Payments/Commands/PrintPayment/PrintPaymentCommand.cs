using AlatrafClinic.Application.Common.Printing.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Commands.PrintPayment;

public sealed record PrintPaymentCommand(int PaymentId) : IRequest<Result<PdfDto>>;
