using AlatrafClinic.Application.Common.Printing;
using AlatrafClinic.Application.Common.Printing.Interfaces;
using AlatrafClinic.Domain.TherapyCards;

namespace AlatrafClinic.Infrastructure.Printing.QuestPDF;

public class TherapyCardPdfGenerator : IPdfGenerator<TherapyCard>
{
    public byte[] Generate(TherapyCard therapyCard, PrintContext context)
    {
        return new byte[0];
    }
}