using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.RepairCards;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.RepairCards.Commands.AssignIndustrialPartToDoctor;
using AlatrafClinic.Application.Features.RepairCards.Commands.AssignRepairCardToDoctor;
using AlatrafClinic.Application.Features.RepairCards.Commands.ChangeRepairCardStatus;
using AlatrafClinic.Application.Features.RepairCards.Commands.CreateDeliveryTime;
using AlatrafClinic.Application.Features.RepairCards.Commands.CreateRepairCard;
using AlatrafClinic.Application.Features.RepairCards.Commands.UpdateRepairCard;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Queries.GetPaidRepairCards;
using AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCardById;
using AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCards;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/repair-cards")]
[ApiVersion("1.0")]
public sealed class RepairCardsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<RepairCardDiagnosisDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of repair cards.")]
    [EndpointDescription("Supports filtering repair cards by various criteria including search term, status, DiagnosisId, IsActive, IsLate, and patient. Sorting is customizable.")]
    [EndpointName("GetRepairCards")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] RepairCardFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetRepairCardsQuery(
            Page: pageRequest.Page,
            PageSize: pageRequest.PageSize,
            SearchTerm: filter.SearchTerm,
            IsActive: filter.IsActive,
            IsLate: filter.IsLate,
            Status: filter.Status,
            DiagnosisId: filter.DiagnosisId,
            PatientId: filter.PatientId,
            SortColumn: filter.SortColumn,
            SortDirection: filter.SortDirection
        );

        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{repairCardId:int}", Name = "GetRepairCardById")]
    [ProducesResponseType(typeof(RepairCardDiagnosisDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a repair card by its ID.")]
    [EndpointDescription("Fetches detailed information about a specific repair card using its unique identifier.")]
    [EndpointName("GetRepairCardById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        int repairCardId,
        CancellationToken ct = default)
    {
       
        var result = await sender.Send(new GetRepairCardByIdQuery(repairCardId), ct);
        
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(RepairCardDiagnosisDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new repair card.")]
    [EndpointDescription("Creates a new repair card with the provided details and returns the created repair card information.")]
    [EndpointName("CreateRepairCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateRepairCardRequest request, CancellationToken ct = default)
    {
        var industrialParts = request.IndustrialParts.ConvertAll(p => new CreateRepairCardIndustrialPartCommand(p.IndustrialPartId, p.UnitId, p.Quantity));
        
        var result = await sender.Send(new CreateRepairCardCommand(
            TicketId: request.TicketId,
            DiagnosisText: request.DiagnosisText,
            request.InjuryDate,
            request.InjuryReasons,
            request.InjurySides,
            request.InjuryTypes,
            industrialParts,
            request.Notes
        ), ct);

         return result.Match(
            response => CreatedAtRoute(
                routeName: "GetRepairCardById",
                routeValues: new { version = "1.0", repairCardId = response.RepairCardId },
                value: response),
                Problem
        );
    }

    [HttpPut("{repairCardId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Update a repair card.")]
    [EndpointDescription("Updates an existing repair card with the provided details.")]
    [EndpointName("UpdateRepairCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(int repairCardId, [FromBody] UpdateRepairCardRequest request, CancellationToken ct = default)
    {
        var industrialParts = request.IndustrialParts.ConvertAll(p => new UpdateRepairCardIndustrialPartCommand(p.IndustrialPartId, p.UnitId, p.Quantity));
        
        var result = await sender.Send(new UpdateRepairCardCommand(
            RepairCardId: repairCardId,
            TicketId: request.TicketId,
            DiagnosisText: request.DiagnosisText,
            request.InjuryDate,
            request.InjuryReasons,
            request.InjurySides,
            request.InjuryTypes,
            industrialParts,
            request.Notes
        ), ct);

         return result.Match(
            response => NoContent(),
                Problem
        );
    }

    [HttpPost("{repairCardId:int}/industrial-part-assignments")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Assign Industrial Part To Doctor.")]
    [EndpointDescription("Assign specific industrial part to specific doctor in specific section")]
    [EndpointName("AssignIndustrialPartToDoctor")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> AssignIndustrialPartToDoctor(int repairCardId, [FromBody] AssignIndustrialPartsRequest request, CancellationToken ct =default)
    {
        var doctorIndustrialParts = request.Assignments.ConvertAll(x=> new DoctorIndustrialPartCommand(x.DiagnosisIndustrialPartId, x.DoctorId, x.SectionId));

        var result = await sender.Send(new AssignIndustrialPartToDoctorCommand(repairCardId, doctorIndustrialParts), ct);

        return result.Match(
            _=> NoContent(),
            Problem
        );
    }

    [HttpPost("{repairCardId:int}/doctor-assignmets")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Assign Repair Card To Doctor")]
    [EndpointDescription("Assign the whole industrial parts in repair card to doctor")]
    [EndpointName("AssignRepairCardToDoctor")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> AssignRepairCardToDoctor(int repairCardId, [FromBody] DoctorAssignmentRequest request, CancellationToken ct =default)
    {
      
        var result = await sender.Send(new AssignRepairCardToDoctorCommand(repairCardId, request.DoctorId, request.SectionId), ct);

        return result.Match(
            _=> NoContent(),
            Problem
        );
    }

    [HttpPatch("{repairCardId:int}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Update a repair card.")]
    [EndpointDescription("Updates an existing repair card with the provided details.")]
    [EndpointName("UpdateRepairCardStatus")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> UpdateStatus(int repairCardId, [FromBody] ChangeRepairCardStatusRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new ChangeRepairCardStatusCommand(repairCardId, request.CardStatus), ct);
      
         return result.Match(
            response => NoContent(),
                Problem
        );
    }

    [HttpPost("{repairCardId:int}/delivery-time")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Create delivery time.")]
    [EndpointDescription("Create delivery time for a repair card.")]
    [EndpointName("CreateDeliveryTime")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> CreateDeliveryTime(int repairCardId, [FromBody] CreateDeliveryTimeRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new CreateDeliveryTimeCommand(repairCardId, request.DeliveryDate, request.Notes), ct);
      
         return result.Match(
            response => NoContent(),
                Problem
        );
    }
    
    [HttpGet("paid")]
    [ProducesResponseType(typeof(PaginatedList<RepairCardDiagnosisDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of paid repair diagnoses.")]
    [EndpointDescription("Returns paid repair/limbs diagnoses that have a repair card. Supports searching by patient name or repair card ID. Sorting is customizable and defaults to PaymentDate asc.")]
    [EndpointName("GetPaidRepairCards")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetPaidRepairCards([FromQuery] GetPaidRepairCardsFilterRequest request, [FromQuery] PageRequest pageRequest, CancellationToken ct = default)
    {
        var query = new GetPaidRepairCardsQuery(
            Page: pageRequest.Page,
            PageSize: pageRequest.PageSize,
            SearchTerm: request.SearchTerm,
            SortColumn: request.SortColumn,
            SortDirection: request.SortDirection
        );

        var result = await sender.Send(query, ct);
        return result.Match(response => Ok(response), Problem);
    }

    [HttpGet("{repairCardId:int}/print")]
    public async Task<IActionResult> Print(int repairCardId, CancellationToken ct = default)
    {
        // For demonstration, we generate a static PDF.
        var pdfBytes = Generate();
        return File(pdfBytes, "application/pdf", $"RepairCard_{repairCardId}.pdf");
    }

    private byte[] Generate()
    {
    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(20);

            page.Content().Column(col =>
            {
                col.Spacing(10);

                // ================= HEADER =================
                col.Item().Row(row =>
                {
                    // Right side (clinic info)
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("مركز الأطراف والعلاج الطبيعي")
                            .FontFamily("Cairo").Bold().AlignRight();
                        c.Item().Text("إدارة الشؤون المالية")
                            .FontFamily("Cairo").AlignRight();
                        c.Item().Text("قسم حسابات الإيرادات")
                            .FontFamily("Cairo").AlignRight();
                    });

                    // Left side (numbers)
                    row.ConstantItem(180).Column(c =>
                    {
                        c.Item().Text("139075").Bold();
                        c.Item().Text("43050");
                        c.Item().Text("29/04/2025");
                    });
                });

                col.Item().LineHorizontal(1);

                // ================= TITLE BOX =================
                col.Item().AlignCenter().Container()
                    .Border(1)
                    .Background(Colors.Grey.Lighten3)
                    .Padding(10)
                    .Column(c =>
                    {
                        c.Item().Text("بطاقة")
                            .FontFamily("Cairo")
                            .Bold()
                            .FontSize(16)
                            .AlignCenter();

                        c.Item().Text("إصلاح فني")
                            .FontFamily("Cairo")
                            .FontSize(14)
                            .AlignCenter();
                    });

                // ================= META INFO =================
                col.Item().Text("بالإدارة الفنية")
                    .FontFamily("Cairo")
                    .AlignRight();

                col.Item().Text("استمارة تحويل رقم : ٥٢٧٢٨٨")
                    .FontFamily("Cairo")
                    .AlignRight();

                col.Item().Text("الأخ رئيس قسم : الحديد")
                    .FontFamily("Cairo")
                    .AlignRight();

                col.Item().Text("يتم سرعة إصلاح :")
                    .FontFamily("Cairo")
                    .AlignRight();

                // ================= TABLE =================
                col.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);   // م
                        columns.RelativeColumn(4);    // التفصيل
                        columns.ConstantColumn(50);   // الكمية
                        columns.ConstantColumn(50);   // الوحدة
                        columns.RelativeColumn(2);    // القسم
                        columns.RelativeColumn(3);    // اسم الفني
                        columns.RelativeColumn(3);    // ملاحظات
                    });

                    void Header(string text) =>
                        table.Cell().Background(Colors.Grey.Lighten2)
                            .Padding(3)
                            .Text(text)
                            .FontFamily("Cairo")
                            .Bold()
                            .AlignCenter();

                    Header("م");
                    Header("التفصيل (نوع الخدمة)");
                    Header("الكمية");
                    Header("الوحدة");
                    Header("القسم");
                    Header("اسم الفني");
                    Header("ملاحظات");

                    void Cell(string text) =>
                        table.Cell().Padding(3)
                            .Text(text)
                            .FontFamily("Cairo")
                            .AlignRight();

                    // Row 1
                    Cell("1");
                    Cell("جهاز حديد مع المفصل يمين");
                    Cell("1");
                    Cell("جهاز");
                    Cell("الحديد");
                    Cell("عبدالعزيز السنجاني");
                    Cell("convannt");

                    // Row 2
                    Cell("2");
                    Cell("جهاز حديد مع المفصل شمال");
                    Cell("1");
                    Cell("جهاز");
                    Cell("الحديد");
                    Cell("عبدالعزيز السنجاني");
                    Cell("");
                });

                // ================= NOTES =================
                col.Item().Text("كونه قد استكمل الإجراءات اللازمة لدينا.")
                    .FontFamily("Cairo")
                    .AlignRight();

                col.Item().Text("الملاحظات : حسب توجيهات الإدارة")
                    .FontFamily("Cairo")
                    .AlignRight();

                // ================= FOOTER =================
                col.Item().LineHorizontal(1);

                col.Item().Row(row =>
                {
                    row.RelativeItem().Text("المختص\nهدى الحمزي")
                        .FontFamily("Cairo")
                        .AlignCenter();

                    row.RelativeItem().Text("رئيس قسم الإيرادات")
                        .FontFamily("Cairo")
                        .AlignCenter();

                    row.RelativeItem().Text("مدير المركز")
                        .FontFamily("Cairo")
                        .AlignCenter();
                });

                col.Item().Text("تاريخ الطباعة : 29/04/2025 12:09")
                    .FontFamily("Cairo")
                    .AlignRight();
            });
        });
    });

    document.ShowInCompanion();
    return document.GeneratePdf();
}

}