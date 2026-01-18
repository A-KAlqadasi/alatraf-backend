using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Sessions;

using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace AlatrafClinic.Infrastructure.Printing.QuestPDF;

public class SessionPdfGenerator : IPdfGenerator<Session>
{
    public byte[] Generate(Session session, PrintContext context)
    {
        var document = Document.Create(container =>
        {
            // ================= PAGE 1 : SUMMARY (PLACEHOLDER) =================
            container.Page(page =>
            {
                page.Size(PageSizes.A7);
                page.Margin(5);
                page.ContentFromRightToLeft();

                page.DefaultTextStyle(t =>
                    t.FontFamily("Cairo")
                    .FontSize(10)
                    .FontColor(AlatrafClinicConstants.DefaultColor));

                page.Content().AlignCenter().Column(col =>
                {
                    col.Spacing(2);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("مركز الأطراف والعلاج الطبيعي");
                        });

                        row.ConstantItem(20)
                        .Image("./Statics/Images/logo.png")
                        .FitArea();
                    });

                    col.Item().LineHorizontal(1);
                    
                    


                    col.Item().Text("ملخص الجلسة")
                        .Bold()
                        .FontSize(12)
                        .AlignCenter();

                    col.Item().Text("سيتم إضافة ملخص الجلسة لاحقاً")
                        .FontSize(12)
                        .AlignCenter();
                });
            });

            // ================= SESSION PROGRAM PAGES =================
            foreach (var program in session.SessionPrograms)
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A7);
                    page.Margin(5);
                    page.ContentFromRightToLeft();

                    page.DefaultTextStyle(t =>
                        t.FontFamily("Cairo")
                        .FontSize(10)
                        .FontColor(AlatrafClinicConstants.DefaultColor));

                    page.Content().Column(col =>
                    {
                        col.Spacing(3);

                        BuildSessionProgramTicket(
                            col,
                            session,
                            program,
                            context);
                    });
                });
            }
        });

        if (PdfDebugSettings.UseCompanion)
            document.ShowInCompanion();

        return document.GeneratePdf();
    }
    private static void BuildSessionProgramTicket(
    ColumnDescriptor col,
    Session session,
    SessionProgram program,
    PrintContext context)
    {
        col.Item().Border(2)
            .Padding(5)
            .Column(card =>
            {
                card.Spacing(2);

                // ================= HEADER =================
                card.Item().Row(row =>
                {
                    row.RelativeItem().AlignRight().Column(c =>
                    {
                        c.Item().Text("مركز الأطراف والعلاج الطبيعي");
                        // c.Item().Text("والعلاج الطبيعي");
                    });

                    row.ConstantItem(20)
                        .Image("./Statics/Images/logo.png")
                        .FitArea();
                });

                card.Item().LineHorizontal(1);

                // ================= TITLE =================
                card.Item().AlignCenter().Text(t =>
                {
                    t.Span("تذكرة جلسة").Bold();
                    t.Span($"  ({session.TherapyCard.Type.ToArabicTherapyCardType()})").FontSize(10);
                    
                    if(context.IsCopy)
                    {
                        t.Span($" - نسخة {context.PrintNumber}");
                    }

                });

                card.Item().LineHorizontal(1);

                // ================= INFO GRID =================
                InfoRow(card, "رقم كرت العلاج", session.TherapyCardId.ToString(), 70);
                InfoRow(card, "الجلسة", session.Number.ToString());
                InfoRow(card, "المريض", session.TherapyCard.Diagnosis.Patient.Person.FullName ?? "—");
                InfoRow(card, "البرنامج","Traininig" ?? "—");
                InfoRow(card, "القسم", program.DoctorSectionRoom?.Section.Name ?? "—");
                InfoRow(card, "الغرفة", program.DoctorSectionRoom?.Room?.Name ?? "—");
                InfoRow(card, "الطبيب", program.DoctorSectionRoom?.Doctor?.Person?.FullName ?? "—");

                card.Item().LineHorizontal(1);

                // ================= FOOTER =================
                card.Item().Text(
                        $"{UtilityService.GetFormattedDateInArabic(context.PrintedAt.Date).ToString()}")
                    .AlignCenter();

                card.Item().Text("شكراً لزيارتكم،،")
                    .Bold()
                    .AlignCenter();
            });
    }
    private static void InfoRow(
    ColumnDescriptor col,
    string label,
    string value, int size = 40)
    {
        col.Item().Row(row =>
        {
            row.ConstantItem(size)
                .Text(label)
                .AlignRight();

            row.RelativeItem()
                .Text(value)
                .Bold()
                .AlignRight();
        });
    }
}