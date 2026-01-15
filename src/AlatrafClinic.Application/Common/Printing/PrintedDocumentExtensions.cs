using AlatrafClinic.Domain.Printing;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Common.Printing;

public static class PrintedDocumentExtensions
{
    public static async Task<PrintedDocument> GetOrCreateAsync(
        this DbSet<PrintedDocument> printedDocuments,
        string documentType,
        int documentId,
        CancellationToken cancellationToken = default)
    {
        var document = await printedDocuments
            .FirstOrDefaultAsync(
                x => x.DocumentType == documentType &&
                     x.DocumentId == documentId,
                cancellationToken);

        if (document is not null)
            return document;

        document = new PrintedDocument(documentType, documentId);
        await printedDocuments.AddAsync(document, cancellationToken);

        return document;
    }
}