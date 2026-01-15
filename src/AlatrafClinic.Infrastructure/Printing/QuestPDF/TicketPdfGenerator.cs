using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Domain.Tickets;

using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AlatrafClinic.Infrastructure.Printing.QuestPDF;

public class TicketPdfGenerator : IPdfGenerator<Ticket>
{
    public byte[] Generate(Ticket ticket, PrintContext context)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A7);
                page.Margin(10);

                page.Content()
                    .Column(col =>
                    {
                        // Clinic name
                        col.Item().Text("مركز الأطراف")
                            .FontFamily("Cairo")
                            .Bold()
                            .FontSize(14)
                            .AlignCenter();

                        // COPY notice
                        if (context.IsCopy)
                        {
                            col.Item().Text($"نسخة – تمت الطباعة {context.PrintNumber} مرات")
                                .FontFamily("Cairo")
                                .FontSize(10)
                                .Bold()
                                .FontColor(Colors.Red.Medium)
                                .AlignCenter();
                        }

                        col.Item().LineHorizontal(1);

                        // Ticket number
                        col.Item().Text($"رقم التذكرة: {ticket.Id}")
                            .FontFamily("Cairo")
                            .AlignRight();

                        // Service
                        col.Item().Text($"الخدمة: {ticket.Service.Name}")
                            .FontFamily("Cairo")
                            .AlignRight();

                        // Price
                        if (ticket.ServicePrice.HasValue)
                        {
                            col.Item().Text($"السعر: {ticket.ServicePrice:0.00}")
                                .FontFamily("Cairo")
                                .AlignRight();
                        }

                        // Patient
                        if (ticket.Patient is not null)
                        {
                            col.Item().Text($"المريض: {ticket.Patient.Person.FullName}")
                                .FontFamily("Cairo")
                                .AlignRight();
                        }

                        // Status
                        col.Item().Text($"الحالة: {TranslateStatus(ticket.Status)}")
                            .FontFamily("Cairo")
                            .AlignRight();

                        // Date
                        col.Item().Text($"التاريخ: {context.PrintedAt:yyyy/MM/dd HH:mm}")
                            .FontFamily("Cairo")
                            .AlignRight();
                            
                    });
                });
            });
            
        if (PdfDebugSettings.UseCompanion)
            document.ShowInCompanion();
        
        return document.GeneratePdf();
    }

    private static string TranslateStatus(TicketStatus status)
    {
        return status switch
        {
            TicketStatus.New => "جديد",
            TicketStatus.Pause => "موقوف مؤقتاً",
            TicketStatus.Continue => "مستمر",
            TicketStatus.Completed => "مكتمل",
            TicketStatus.Cancelled => "ملغي",
            _ => status.ToString()
        };
    }

}
public static class PdfDebugSettings
{
    public static bool UseCompanion = true; // DEV ONLY
}