
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.RepairCards;


using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AlatrafClinic.Infrastructure.Printing.QuestPDF;


public class RepairCardPdfGenerator(IUser user, IIdentityService identityService) : IPdfGenerator<RepairCard>
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
                    .FontColor(AlatrafClinicConstants.DefaultColor));

                page.Content().Column(col =>
                {
                    col.Spacing(10);
                    
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
                    
                    // ================= TITLE BLOCK =================
                    col.Item().PaddingBottom(5).Column(col =>
                    {
                        col.Item().PaddingBottom(5).Row(row =>
                        {
                            
                            row.RelativeItem().AlignRight()
                                .Text(t =>
                                {
                                    t.Span("التاريخ: ");
                                    t.Span(context.PrintedAt.ToString(" dd/MM/yyyy")).Bold();
                                });
                            

                            row.RelativeItem().AlignCenter().Text(t=>
                            {
                                t.Span("كرت إصلاح فني").Bold().FontSize(16);
                                if (context.PrintNumber > 1)
                                {
                                    t.Span($"   - نسخة رقم {context.PrintNumber}").FontSize(9);
                                }
                            });
                                
                            row.RelativeItem().AlignLeft()
                                .Text(t=>
                                {
                                    t.Span("رقم الكرت: ");
                                    t.Span(repairCard.Id.ToString()).Bold();
                                });
                        });

                        col.Item().Row(row =>
                        {
                            
                            row.RelativeItem().AlignRight()
                                .Text(t=>
                                {
                                    t.Span("الأخ رئيس قسم: ");
                                    t.Span(string.Join(", ", GetSections(repairCard))).Bold();
                                    t.Span("     المحترم");
                                });
                            
                            row.RelativeItem().AlignLeft()
                                 .Text(t=>
                                 {
                                     t.Span("موعد التسليم: ");
                                     t.Span(repairCard.DeliveryTime?.DeliveryDate.ToString("dd/MM/yyyy")).Bold();
                                 });

                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight()
                                .Text(t =>
                                {
                                    t.Span("يتم سرعة إصلاح للأخ/الأخت: ");
                                    t.Span(repairCard.Diagnosis.Patient?.Person.FullName ?? "غير معروف").Bold();
                                });

                            row.RelativeItem().AlignLeft()
                                .Text(t =>
                                {
                                    t.Span("رقم المريض : ");
                                    t.Span(repairCard.Diagnosis.Patient.Id.ToString()).Bold();
                                });

                        });
                    });

                    
                    // ================= TABLE =================
                    col.Item().PaddingBottom(5).Table(table =>
                    {
                        
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);  // م
                            columns.RelativeColumn(3);   // القطع الصناعية
                            columns.ConstantColumn(50);  // الكمية
                            columns.ConstantColumn(80);  // الوحدة
                            columns.RelativeColumn(3);   // اسم الفني
                            columns.ConstantColumn(80);   // القسم
                        });

                        void Header(string text) =>
                            table.Cell()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(5)
                                .Text(text)
                                .Bold()
                                .AlignRight();

                        Header("م");
                        Header("القطع الصناعية");
                        Header("الكمية");
                        Header("الوحدة");
                        Header("اسم الفني");
                        Header("القسم");

                        void Cell(string text, Color background)
                        {
                            table.Cell()
                                .Background(background)
                                .Padding(5)
                                .Text(text)
                                .AlignRight();
                        }

                        int index = 1;

                        foreach (var item in repairCard.DiagnosisIndustrialParts)
                        {
                            var rowBackground = index % 2 == 0
                                ? Colors.Grey.Lighten4   // stripe color
                                : Colors.White;          // normal row
                           
                            Cell(index.ToString(), rowBackground);
                            Cell(item.IndustrialPartUnit.IndustrialPart.Name, rowBackground);                            
                            Cell(item.Quantity.ToString(), rowBackground);
                            Cell(item.IndustrialPartUnit.Unit?.Name ?? string.Empty, rowBackground);
                            Cell(item.DoctorSectionRoom?.Doctor?.Person?.FullName ?? "غير معروف", rowBackground);
                            Cell(item.DoctorSectionRoom?.Section.Name ?? string.Empty, rowBackground);
                            index++;
                        }
                                                
                    });

                    // ================= NOTES =================
                    
                    col.Item().Row(row =>
                    {
                        
                        row.RelativeItem().AlignRight()
                            .Text(t =>
                            {
                                t.Span("رقم السند: ");
                                t.Span(repairCard.Diagnosis.Payments.FirstOrDefault()?.PatientPayment?.VoucherNumber.ToString() ?? "").Bold();
                            });

                        row.RelativeItem().AlignCenter()
                            .Text(t=>
                            {
                                t.Span("الاجمالي: ");
                                t.Span(repairCard.Diagnosis.Payments.Sum(p=> p.TotalAmount).ToString()).Bold();
                            });

                        row.RelativeItem().AlignLeft()
                            .Text(t=>
                            {
                                t.Span("التخفيض: ");
                                t.Span(repairCard.Diagnosis.Payments.Sum(p=> p.DiscountAmount).ToString()).Bold();
                            });
                        
                    });

                    col.Item().LineHorizontal(1);

                    // ================= FOOTER =================
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().AlignRight()
                            .Text(async t=>
                            {
                                t.Span("المستخدم: ");
                                t.Span(await GetUserName()).Bold();
                            });

                        row.RelativeItem().AlignCenter()
                            .Text("رئيس قسم الإيرادات");

                        row.RelativeItem().PaddingLeft(20).AlignLeft()
                            .Text("مدير المركز");
                    });
                });
            });
        });
        

        if (PdfDebugSettings.UseCompanion)
            document.ShowInCompanion(); // DEV ONLY

        return document.GeneratePdf();
    }
    

    private List<string> GetSections(RepairCard repairCard)
    {
        var sections = new List<string>();
        foreach (var industrialPart in repairCard.DiagnosisIndustrialParts)
        {
            var section = industrialPart.DoctorSectionRoom?.Section.Name; 
            if (section is null)
                continue;
            
            if (!sections.Contains(section))
                sections.Add(section);

        }
        return sections;
    }

    private async Task<string> GetUserName()
    {
        // 19a59129-6c20-417a-834d-11a208d32d96
        return await identityService.GetUserFullNameAsync(user.Id ?? "");
    }



}