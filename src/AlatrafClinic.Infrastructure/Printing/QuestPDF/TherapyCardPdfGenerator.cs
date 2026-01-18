using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.Enums;

using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AlatrafClinic.Infrastructure.Printing.QuestPDF;

public class TherapyCardPdfGenerator(IUser user, IIdentityService identityService) : IPdfGenerator<TherapyCard>
{
    public byte[] Generate(TherapyCard therapyCard, PrintContext context)
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
                                t.Span($"كرت علاج طبيعي - {therapyCard.Type.ToArabicTherapyCardType()}").Bold().FontSize(16);

                                if (context.PrintNumber > 1)
                                {
                                    t.Span($"   - نسخة رقم {context.PrintNumber}").FontSize(9);
                                }
                            });
                                
                            row.RelativeItem().AlignLeft()
                                .Text(t=>
                                {
                                    t.Span("رقم الكرت: ");
                                    t.Span(therapyCard.Id.ToString()).Bold();
                                });
                        });

                        
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight()
                                .Text(t =>
                                {
                                    t.Span("اسم المريض: ");
                                    t.Span(therapyCard.Diagnosis.Patient?.Person.FullName ?? "—").Bold();
                                });

                            row.RelativeItem().AlignLeft()
                                .Text(t =>
                                {
                                    t.Span("رقم المريض: ");
                                    t.Span(therapyCard.Diagnosis.Patient.Id.ToString()).Bold();
                                });

                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight()
                                .Text(t =>
                                {
                                    t.Span("سبب الإصابة : ");
                                    t.Span(string.Join(", ", GetInjuryReasons(therapyCard.Diagnosis))).Bold();
                                });

                            row.RelativeItem().AlignCenter()
                                .Text(t =>
                                {
                                    t.Span("نوع الإصابة : ");
                                    t.Span(string.Join(", ", GetInjuryTypes(therapyCard.Diagnosis))).Bold();
                                });

                             row.RelativeItem().AlignLeft()
                                .Text(t =>
                                {
                                    t.Span("جهة الإصابة : ");
                                    t.Span(string.Join(", ", GetInjurySides(therapyCard.Diagnosis))).Bold();
                                });

                        });

                        col.Item().PaddingBottom(5).Row(row =>
                        {
                            
                            row.RelativeItem(3)
                                .Text(t =>
                                {
                                    t.Span("مدة الإصابة: ");
                                    t.Span(UtilityService.CalculateDurationArabic(therapyCard.Diagnosis.InjuryDate).ToString()).Bold();
                                });

                            row.RelativeItem(2)
                                .Text(t =>
                                {
                                    t.Span("بداية البرنامج: ");
                                    t.Span(therapyCard.ProgramStartDate.ToString("dd/MM/yyyy")).Bold();
                                });
                            
                            if(therapyCard.Type != TherapyCardType.Special)
                            {
                                row.RelativeItem(2).AlignLeft()
                                .Text(t =>
                                {
                                    t.Span("نهاية البرنامج: ");
                                    t.Span(therapyCard.ProgramEndDate?.ToString("dd/MM/yyyy")).Bold();
                                });
                            } 
                            else
                            {
                                row.RelativeItem(2).AlignLeft()
                                .Text(t =>
                                {
                                    t.Span("عدد الجلسات: ");
                                    t.Span(therapyCard.NumberOfSessions.ToString()).Bold();
                                });
                            }

                        });

                        col.Item().LineHorizontal(1);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().AlignLeft()
                                .Text(t =>
                                {
                                    row.RelativeItem(1).AlignMiddle().AlignRight()
                                    .Text("التشخيص ").Bold();
                                    row.RelativeItem(10).AlignLeft()
                                    .Text(t =>
                                    {
                                    t.Span(therapyCard.Diagnosis.DiagnosisText ?? "—").Bold();
                                    });
                                });
                        });

                    });

                    
                    // ================= TABLE =================
                    col.Item().PaddingBottom(5).Table(table =>
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

                        foreach (var item in therapyCard.DiagnosisPrograms)
                        {
                            var rowBackground = index % 2 == 0
                                ? Colors.Grey.Lighten4   // stripe color
                                : Colors.White;          // normal row
                           
                            Cell(index.ToString(), rowBackground);
                            Cell(item.MedicalProgram?.Name ?? "—", rowBackground);                            
                            Cell(item.Duration.ToString(), rowBackground);
                            Cell(item.MedicalProgram?.Section?.Name ?? "—", rowBackground);
                            Cell(item.Notes ?? "", rowBackground);

                            
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
                                t.Span(therapyCard.Diagnosis.Payments.FirstOrDefault()?.PatientPayment?.VoucherNumber.ToString() ?? "").Bold();
                            });

                        row.RelativeItem().AlignCenter()
                            .Text(t=>
                            {
                                t.Span("الاجمالي: ");
                                t.Span(therapyCard.Diagnosis.Payments.Sum(p=> p.TotalAmount).ToString()).Bold();
                            });

                        row.RelativeItem().AlignLeft()
                            .Text(t=>
                            {
                                t.Span("التخفيض: ");
                                t.Span(therapyCard.Diagnosis.Payments.Sum(p=> p.DiscountAmount).ToString()).Bold();
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

    private async Task<string> GetUserName()
    {
        // 19a59129-6c20-417a-834d-11a208d32d96
        return await identityService.GetUserFullNameAsync(user.Id ?? "");
    }

}