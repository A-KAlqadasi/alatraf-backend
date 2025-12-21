
using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.Payments;
using AlatrafClinic.Application.Features.Payments.Commands.PayPayments;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Queries.GetPayment;
using AlatrafClinic.Application.Features.Payments.Queries.GetPaymentsWaitingList;
using AlatrafClinic.Application.Features.Payments.Queries.GetRepairPayment;
using AlatrafClinic.Application.Features.Payments.Queries.GetTherapyPayment;
using AlatrafClinic.Domain.Payments;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/payments")]
[ApiVersion("1.0")]
public sealed class PaymentsController(ISender sender) : ApiController
{
    
    [HttpGet("{paymentId:int}", Name = "GetPaymentById")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a payment by its ID.")]
    [EndpointDescription("Returns detailed information about the specified payment if it exists.")]
    [EndpointName("GetPaymentById")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(int paymentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentQuery(paymentId), ct);
        
        return result.Match(
          response => Ok(response),
          Problem);
    }

    [HttpGet("waiting-list", Name = "GetPaymentsWaitingList")]
    [ProducesResponseType(typeof(PaymentWaitingListDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves paymets waiting list")]
    [EndpointDescription("Returns paymets for waiting list for completing payment process")]
    [EndpointName("GetPaymentsWaitingList")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetPaymentsWaitingList([FromQuery] GetPaymentsWaitingListFilterRequest filter , [FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        var result = await sender.Send(new GetPaymentsWaitingListQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SearchTerm,
            filter.PaymentReference is not null ? (PaymentReference)(int)filter.PaymentReference : null,
            filter.IsCompleted,
            filter.SortColumn,
            filter.SortDirection
         ), ct);
        
        return result.Match(
          response => Ok(response),
          Problem);
    }

    [HttpGet("therapy-payments/{paymentId:int}/payment-reference/{paymentReference}", Name = "GetTherapyPaymentByIdAndReference")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a therapy payment by its ID and payment reference.")]
    [EndpointDescription("Returns detailed information about the specified therapy payment if it exists. payment References are {TherapyCardNew, TherapyCardRenew, TherapyCardDamagedReplacement}.")]
    [EndpointName("GetTherapyPaymentByIdAndReference")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetTherapyPaymentById(int paymentId, PaymentReference paymentReference, CancellationToken ct)
    {
        var result = await sender.Send(new GetTherapyPaymentQuery(paymentId, paymentReference), ct);
        
        return result.Match(
          response => Ok(response),
          Problem);
    }

    [HttpGet("repair-payments/{paymentId:int}/payment-reference/{paymentReference}", Name = "GetRepairPaymentByIdAndReference")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a repair payment by its ID and payment reference.")]
    [EndpointDescription("Returns detailed information about the specified repair payment if it exists. payment Reference is {Repair}.")]
    [EndpointName("GetRepairPaymentByIdAndReference")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetRepairPaymentById(int paymentId, PaymentReference paymentReference, CancellationToken ct)
    {
        var result = await sender.Send(new GetRepairPaymentQuery(paymentId, paymentReference), ct);
        
        return result.Match(
          response => Ok(response),
          Problem);
    }

    [HttpPost("{paymentId:int}/pay/free")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Pays a payment as Free.")]
    [EndpointDescription("Marks the payment as completed with AccountKind=Free. PaidAmount and Discount are stored as null.")]
    [EndpointName("PayFreePayment")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> PayFree(int paymentId, [FromBody] PayFreePaymentRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new PayFreePaymentCommand(paymentId), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{paymentId:int}/pay/patient")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Pays a payment as Patient.")]
    [EndpointDescription("Completes the payment with paid amount/discount and stores PatientPayment details (VoucherNumber, Notes).")]
    [EndpointName("PayPatientPayment")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> PayPatient(int paymentId, [FromBody] PayPatientPaymentRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new PayPatientPaymentCommand(
            paymentId,
            request.PaidAmount,
            request.Discount,
            request.VoucherNumber,
            request.Notes
        ), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{paymentId:int}/pay/disabled")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Pays a payment as Disabled.")]
    [EndpointDescription("Completes the payment as Disabled (PaidAmount/Discount null) and stores DisabledPayment details (DisabledCardId, Notes).")]
    [EndpointName("PayDisabledPayment")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> PayDisabled(int paymentId, [FromBody] PayDisabledPaymentRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new PayDisabledPaymentCommand(
            paymentId,
            request.DisabledCardId,
            request.Notes
        ), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{paymentId:int}/pay/wounded")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Pays a payment as Wounded.")]
    [EndpointDescription("Completes the payment as Wounded (PaidAmount/Discount null) and stores WoundedPayment details (WoundedCardId, ReportNumber, Notes). ReportNumber may be required depending on payment total.")]
    [EndpointName("PayWoundedPayment")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> PayWounded(int paymentId, [FromBody] PayWoundedPaymentRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new PayWoundedPaymentCommand(
            paymentId,
            request.WoundedCardId,
            request.ReportNumber,
            request.Notes
        ), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

}