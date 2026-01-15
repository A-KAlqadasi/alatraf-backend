using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.Printing;

public class PrintedDocument : AuditableEntity<Guid>
{
    public string DocumentType { get; private set; } = default!;
    public int DocumentId { get; private set; }
    public int PrintCount { get; private set; }

    private PrintedDocument() { }

    public PrintedDocument(string documentType, int documentId)
    {
        DocumentType = documentType;
        DocumentId = documentId;
        PrintCount = 0;
    }

    public int RegisterPrint()
    {
        PrintCount++;
        return PrintCount;
    }

    public bool IsCopy => PrintCount > 1;
}