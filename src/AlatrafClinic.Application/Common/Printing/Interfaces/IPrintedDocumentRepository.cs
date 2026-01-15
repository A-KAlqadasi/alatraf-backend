using AlatrafClinic.Domain.Printing;

namespace AlatrafClinic.Application.Common.Printing.Interfaces;

public interface IPrintedDocumentRepository
{
    Task<PrintedDocument> GetOrCreateAsync(
        string documentType,
        int documentId,
        CancellationToken ct = default);
}