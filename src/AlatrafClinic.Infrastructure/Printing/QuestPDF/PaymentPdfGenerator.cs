
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.TherapyCards.Enums;


using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AlatrafClinic.Infrastructure.Printing.QuestPDF;


public class PaymentPdfGenerator(IUser user, IIdentityService identityService) : IPdfGenerator<Payment>
{
    
    public byte[] Generate(Payment payment, PrintContext context)
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
                    col.Item().PaddingBottom(10).Column(col =>
                    {
                        col.Item().PaddingBottom(10).Row(row =>
                        {
                            
                            row.RelativeItem().AlignRight()
                                .Text(t =>
                                {
                                    t.Span("التاريخ: ");
                                    t.Span(context.PrintedAt.ToString(" dd/MM/yyyy")).Bold();
                                });
                            

                            row.RelativeItem().AlignCenter().Text(t=>
                            {
                                t.Span($"إستمارة تحويل").Bold().FontSize(16);

                                if (context.PrintNumber > 1)
                                {
                                    t.Span($"   - نسخة رقم {context.PrintNumber}").FontSize(12);
                                }
                            });
                                
                            row.RelativeItem().AlignLeft()
                                .Text(t=>
                                {
                                    t.Span("رقم الاستمارة: ");
                                    t.Span(payment.Id.ToString()).Bold();
                                });
                        });

                        
                        col.Item().Row(row =>
                        {
                            row.RelativeItem(2)
                                .Text(t =>
                                {
                                    t.Span("اسم المريض: ");
                                    t.Span(payment.Diagnosis.Patient?.Person.FullName ?? "غير معروف").Bold();
                                });

                            row.RelativeItem(1)
                                .Text(t =>
                                {
                                    t.Span("رقم المريض: ");
                                    t.Span(payment.Diagnosis.Patient.Id.ToString()).Bold();
                                });
                                
                            row.RelativeItem(1)
                                .Text(t =>
                                {
                                    t.Span("الجنس: ");
                                    t.Span(UtilityService.GenderToArabicString(payment.Diagnosis.Patient.Person.Gender)).Bold();
                                });

                             row.RelativeItem(1)
                                .Text(t =>
                                {
                                    t.Span("العمر: ");
                                    t.Span(payment.Diagnosis.Patient.Person.Age.ToString()).Bold();
                                });
                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight()
                                .Text(t =>
                                {
                                    t.Span("سبب الإصابة : ");
                                    t.Span(string.Join(", ", GetInjuryReasons(payment.Diagnosis))).Bold();
                                });

                            row.RelativeItem().AlignCenter()
                                .Text(t =>
                                {
                                    t.Span("نوع الإصابة : ");
                                    t.Span(string.Join(", ", GetInjuryTypes(payment.Diagnosis))).Bold();
                                });

                             row.RelativeItem().AlignLeft()
                                .Text(t =>
                                {
                                    t.Span("جهة الإصابة : ");
                                    t.Span(string.Join(", ", GetInjurySides(payment.Diagnosis))).Bold();
                                });

                        });

                        col.Item().PaddingBottom(20).Row(row =>
                        {
                            
                            row.RelativeItem(3)
                                .Text(t =>
                                {
                                    t.Span("مدة الإصابة: ");
                                    t.Span(UtilityService.CalculateDurationArabic(payment.Diagnosis.InjuryDate).ToString()).Bold();
                                });

                            if(payment.PaymentReference != PaymentReference.Repair)
                            {
                                row.RelativeItem(2)
                                .Text(t =>
                                {
                                    t.Span("نوع البرنامج: ");
                                    t.Span(payment.Diagnosis?.TherapyCard?.Type.ToArabicTherapyCardType() ?? "").Bold();
                                });

                                row.RelativeItem(2)
                                .Text(t =>
                                {
                                    t.Span("بداية البرنامج: ");
                                    t.Span(payment.Diagnosis?.TherapyCard?.ProgramStartDate.ToString("dd/MM/yyyy")).Bold();
                                });
                                
                                if(payment.Diagnosis?.TherapyCard?.Type != TherapyCardType.Special)
                                {
                                    row.RelativeItem(2).AlignLeft()
                                    .Text(t =>
                                    {
                                        t.Span("نهاية البرنامج: ");
                                        t.Span(payment.Diagnosis?.TherapyCard?.ProgramEndDate?.ToString("dd/MM/yyyy")).Bold();
                                    });
                                } 
                                else
                                {
                                    row.RelativeItem(2).AlignLeft()
                                    .Text(t =>
                                    {
                                        t.Span("عدد الجلسات: ");
                                        t.Span(payment.Diagnosis?.TherapyCard?.NumberOfSessions.ToString()).Bold();
                                    });
                                }
                            }

                        });

                        col.Item().LineHorizontal(1);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem(1).AlignMiddle().AlignRight()
                                .Text("التشخيص ").Bold();
                            row.RelativeItem(10).AlignLeft()
                                .Text(t =>
                                {
                                   t.Span(payment.Diagnosis.DiagnosisText ?? "لا يوجد").Bold();
                                });
                        });

                    });

                    
                    // ================= TABLE =================
                    if(payment.PaymentReference == PaymentReference.Repair)
                    {
                        col.Item().Element(c =>
                        {
                            BuildIndustrialPartsTable(c, payment.Diagnosis.DiagnosisIndustrialParts);
                        });
                    }
                    else
                    {
                        col.Item().Element(c =>
                        {
                            BuildMedicalProgramsTable(c, payment.Diagnosis.DiagnosisPrograms);
                        });
                    }

                    col.Item().PaddingTop(10).Row(row =>
                    {
                        row.RelativeItem().AlignRight()
                            .Text(t=>
                            {
                                t.Span("الاجمالي: ");
                                t.Span(payment.TotalAmount.ToString("0.##")).Bold();
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
    
    private async Task<string> GetUserName()
    {
        // 19a59129-6c20-417a-834d-11a208d32d96
        return await identityService.GetUserFullNameAsync(user.Id ?? "");
    }

    private static void BuildIndustrialPartsTable(
    IContainer container,
    IReadOnlyCollection<DiagnosisIndustrialPart> parts)
    {
        container.PaddingBottom(5).Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(30);  // م
                columns.RelativeColumn(2);   // القطع الصناعية
                columns.ConstantColumn(50);  // الكمية
                columns.ConstantColumn(90);  // الوحدة
                columns.ConstantColumn(90);   // االقيمة
                columns.ConstantColumn(90);  // الاجمالي
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
            Header("القيمة");
            Header("الاجمالي");

            void Cell(string text, Color background)
            {
                table.Cell()
                    .Background(background)
                    .Padding(5)
                    .Text(text)
                    .AlignRight();
            }

            int index = 1;

            foreach (var item in parts)
            {
                var rowBackground = index % 2 == 0
                    ? Colors.Grey.Lighten4
                    : Colors.White;

                Cell(index.ToString(), rowBackground);
                Cell(item.IndustrialPartUnit.IndustrialPart.Name, rowBackground);
                Cell(item.Quantity.ToString(), rowBackground);
                Cell(item.IndustrialPartUnit.Unit?.Name ?? string.Empty, rowBackground);
                Cell(item.IndustrialPartUnit.PricePerUnit.ToString("0.##") ?? "غير معروف", rowBackground);
                Cell(item.Price.ToString("0.##") ?? string.Empty, rowBackground);

                index++;
            }
        });
    }

     private static void BuildMedicalProgramsTable(
    IContainer container,
    IReadOnlyCollection<DiagnosisProgram> programs)
    {
        container.PaddingBottom(5).Table(table =>
                    {
                        
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(30);  // م
                            columns.RelativeColumn(4);   // القطع الصناعية
                            columns.ConstantColumn(50);  // الزمن
                            columns.ConstantColumn(80);  // القسم
                            columns.RelativeColumn(3);   // اسم الفني
                        });

                        void Header(string text) =>
                            table.Cell()
                                .Background(Colors.Grey.Lighten3)
                                .Padding(5)
                                .Text(text)
                                .Bold()
                                .AlignRight();

                        Header("م");
                        Header("البرامج العلاجية");
                        Header("الزمن");
                        Header("القسم");
                        Header("ملاحظات");

                        void Cell(string text, Color background)
                        {
                            table.Cell()
                                .Background(background)
                                .Padding(5)
                                .Text(text)
                                .AlignRight();
                        }

                        int index = 1;

                        foreach (var item in programs)
                        {
                            var rowBackground = index % 2 == 0
                                ? Colors.Grey.Lighten4   // stripe color
                                : Colors.White;          // normal row
                           
                            Cell(index.ToString(), rowBackground);
                            Cell(item.MedicalProgram?.Name ?? "", rowBackground);                            
                            Cell(item.Duration.ToString(), rowBackground);
                            Cell(item.MedicalProgram?.Section?.Name ?? string.Empty, rowBackground);
                            Cell(item.Notes ?? string.Empty, rowBackground);

                            
                            index++;
                        }
                                                
                    });
    }

    private List<string> GetInjuryReasons(Diagnosis diagnosis)
    {
        var reasons = new List<string>();
        foreach (var reason in diagnosis.InjuryReasons)
        {
            
            if (!reasons.Contains(reason.Name))
                reasons.Add(reason.Name);
        }
        return reasons;
    }
    private List<string> GetInjuryTypes(Diagnosis diagnosis)
    {
        var types = new List<string>();
        foreach (var reason in diagnosis.InjuryTypes)
        {
            
            if (!types.Contains(reason.Name))
                types.Add(reason.Name);
        }
        return types;
    }
     private List<string> GetInjurySides(Diagnosis diagnosis)
    {
        var sides = new List<string>();
        foreach (var side in diagnosis.InjurySides)
        {
            if (!sides.Contains(side.Name))
                sides.Add(side.Name);
        }
        return sides;
    }

}