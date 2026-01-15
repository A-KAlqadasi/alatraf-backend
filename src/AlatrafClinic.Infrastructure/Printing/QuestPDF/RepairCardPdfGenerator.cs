using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Domain.RepairCards;

using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace AlatrafClinic.Infrastructure.Printing.QuestPDF;

public class RepairCardPdfGenerator : IPdfGenerator<RepairCard>
{
    
    public byte[] Generate(RepairCard repairCard, PrintContext context)
    {
         var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.ContentFromRightToLeft();

                page.DefaultTextStyle(t =>
                    t.FontFamily("Cairo")
                    .FontSize(11)
                    .FontColor(Colors.Blue.Darken2));

                page.Content().Column(col =>
                {
                    col.Spacing(12);

                    // ================= HEADER =================
                    col.Item().Row(row =>
                    {
                        // RIGHT: Arabic header
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("الجمهورية اليمنية").Bold();
                            c.Item().Text("وزارة الصحة والسكان");
                            c.Item().Text("مركز الأطراف والعلاج الطبيعي");
                        });

                        // CENTER: Logo
                        row.ConstantItem(60)
                            .AlignCenter()
                            .Image("./Statics/Images/logo.png")
                            .FitArea();

                        // LEFT: English header
                        row.RelativeItem().AlignLeft().Column(c =>
                        {
                            c.Item().Text("Republic of Yemen").Bold().AlignLeft();
                            c.Item().Text("Ministry of Health & Population").AlignLeft();
                            c.Item().Text("Physiotherapy & Prosthesis Center").AlignLeft();
                        });
                    });

                    col.Item().LineHorizontal(1);

                    // ================= TITLE =================
                    col.Item().Text("بطاقة إصلاح فني")
                        .Bold()
                        .FontSize(14)
                        .AlignCenter();

                    // ================= META INFO =================
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().AlignRight()
                            .Text("رقم البطاقة : 139075");

                        row.RelativeItem().AlignCenter()
                            .Text("بالإدارة الفنية المحترم");

                        row.RelativeItem().AlignLeft()
                            .Text("التاريخ : 2026-01-17");
                    });

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().AlignRight()
                            .Text("رقم : 43050");

                        row.RelativeItem().AlignCenter()
                            .Text("الأخ رئيس قسم : الحديد");

                        row.RelativeItem().AlignLeft()
                            .Text("يتم سرعة إصلاح للأخ : أيمن أحمد محمد مرعي");
                    });

                    // ================= TABLE =================
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);  // م
                            columns.RelativeColumn(4);   // القطع الصناعية
                            columns.ConstantColumn(60);  // الكمية
                            columns.ConstantColumn(60);  // الوحدة
                            columns.RelativeColumn(3);   // اسم الفني
                            columns.RelativeColumn(3);   // ملاحظات
                        });

                        void Header(string text) =>
                            table.Cell()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(5)
                                .Text(text)
                                .Bold()
                                .AlignCenter();

                        Header("م");
                        Header("القطع الصناعية");
                        Header("الكمية");
                        Header("الوحدة");
                        Header("اسم الفني");
                        Header("ملاحظات");

                        void Cell(string text) =>
                            table.Cell()
                                .Padding(5)
                                .Text(text)
                                .AlignRight();

                        // Row 1
                        Cell("1");
                        Cell("جهاز حديد مع المفصل يمين");
                        Cell("1");
                        Cell("جهاز");
                        Cell("عبدالعزيز السنجاني");
                        Cell("");

                        // Row 2
                        Cell("2");
                        Cell("جهاز حديد مع المفصل شمال");
                        Cell("1");
                        Cell("جهاز");
                        Cell("عبدالعزيز السنجاني");
                        Cell("");
                    });

                    // ================= NOTES =================
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().AlignRight()
                            .Text("التخفيض");

                        row.RelativeItem().AlignCenter()
                            .Text("الاجمالي");

                        row.RelativeItem().AlignLeft()
                            .Text("رقم السند : 000000").Bold();
                    });

                    col.Item().LineHorizontal(1);

                    // ================= FOOTER =================
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().AlignCenter()
                            .Text("مدير المركز");

                        row.RelativeItem().AlignCenter()
                            .Text("رئيس قسم الإيرادات");

                        row.RelativeItem().AlignCenter()
                            .Text("المختص : هدى الحمزي").Bold();
                    });
                });
            });
        });

        if (PdfDebugSettings.UseCompanion)
            document.ShowInCompanion(); // DEV ONLY
        return document.GeneratePdf();
    }

}