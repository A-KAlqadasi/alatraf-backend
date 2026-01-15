
namespace AlatrafClinic.Application.Common.Printing;

public sealed class PrintContext
{
    public int PrintNumber { get; init; }
    public bool IsCopy => PrintNumber > 1;
    public DateTime PrintedAt { get; init; }
}
