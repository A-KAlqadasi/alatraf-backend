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
                page.Size(PageSizes.A5);
                page.ContentFromRightToLeft();
                page.Margin(10);

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    // ================= HEADER =================
                    col.Item().Row(row =>
                    {
                        // RIGHT: Clinic info
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("مركز الأطراف والعلاج الطبيعي")
                                .FontFamily("Cairo")
                                .Bold()
                                .AlignRight();

                            c.Item().Text("إدارة الشؤون المالية")
                                .FontFamily("Cairo")
                                .AlignRight();

                            c.Item().Text("قسم حسابات الإيرادات")
                                .FontFamily("Cairo")
                                .AlignRight();
                        });

                        // CENTER: Title box
                        row.RelativeItem().AlignCenter()
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

                        // LEFT: Numbers
                        row.RelativeItem().AlignLeft().Column(c =>
                        {
                            c.Item().Text("139075").Bold();
                            c.Item().Text("43050");
                            c.Item().Text("29/04/2025");
                        });
                    });
                    col.Item().LineHorizontal(1);                    

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