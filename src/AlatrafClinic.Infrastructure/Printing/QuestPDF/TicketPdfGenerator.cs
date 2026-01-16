using System.Drawing;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Tickets;

using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

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
                page.Margin(8);
                page.ContentFromRightToLeft();

                
                page.DefaultTextStyle(text =>
                    text.FontFamily("Cairo")
                        .FontColor(AlatrafClinicConstants.DefaultColor) // 👈 DEFAULT COLOR
                        .FontSize(9));

                page.Content().Border(2).BorderColor(AlatrafClinicConstants.DefaultColor)
                    .Padding(8)
                    .Column(col =>
                    {
                        col.Spacing(6);

                        // ================= HEADER =================
                        col.Item().Row(row =>
                        {
                            row.Spacing(26);
                            row.ConstantItem(35)
                                .Image("./Statics/Images/logo.png")
                                .FitArea();

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("مـركـــز الأطـــراف")
                                    .FontFamily("Cairo")
                                    .Bold()
                                    .AlignRight();

                                c.Item().Text("والعلاج الطـبيعي")
                                    .FontFamily("Cairo")
                                    .Bold()
                                    .AlignRight();
                            });
                        });

                        col.Item().LineHorizontal(1).LineColor(AlatrafClinicConstants.DefaultColor);

                        // ================= TITLE =================
                        if(context.PrintNumber <= 1)
                        {
                            col.Item().Text("تذكرة خدمة")
                            .FontFamily("Cairo")
                            .Bold()
                            .AlignCenter();
                        }
                        else
                        {
                            col.Item().Text($"تذكرة خدمة - نسخة {context.PrintNumber} ")
                            .FontFamily("Cairo")
                            .Bold()
                            .AlignCenter();
                        }
                        

                        col.Item().LineHorizontal(1).LineColor(AlatrafClinicConstants.DefaultColor);

                        // ================= DETAILS =================
                        void InfoRow(string label, string value)
                        {
                            col.Item().Row(row =>
                            {
                                row.ConstantItem(60)
                                    .Text(label)
                                    .FontFamily("Cairo")
                                    .AlignRight();

                                row.RelativeItem()
                                    .Text(value)
                                    .FontFamily("Cairo")
                                    .Bold()
                                    .AlignRight();
                            });
                        }

                        InfoRow("رقم التذكرة", ticket.Id.ToString());
                        if(ticket.Patient != null)
                        {
                            InfoRow("المريض", ticket.Patient.Person.FullName);
                            
                        }
                        InfoRow("الخدمة", ticket.Service.Name);
                        InfoRow("الحالة", TranslateStatus(ticket.Status));

                        col.Item().LineHorizontal(1).LineColor(AlatrafClinicConstants.DefaultColor);

                        var formattedDate = UtilityService.GetFormattedDateInArabic(context.PrintedAt);

                        // ================= FOOTER =================
                        col.Item().Text(formattedDate)
                            .FontFamily("Cairo")
                            .AlignCenter();

                        col.Item().Text("شكراً لزيارتكم ,,")
                            .FontFamily("Cairo")
                            .Bold()
                            .AlignCenter();
                    });
            });
        });
        
        if (PdfDebugSettings.UseCompanion)
            document.ShowInCompanion(); // DEV ONLY
            
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
