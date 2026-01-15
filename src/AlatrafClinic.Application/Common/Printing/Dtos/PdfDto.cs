namespace AlatrafClinic.Application.Common.Printing.Dtos;

public sealed class PdfDto
{
    public byte[] Content { get; init; } = null!;
    public string FileName { get; init; } = null!;
    public string ContentType { get; init; } = "application/pdf";
}