using System.Text;

using AlatrafClinic.Application.Reports.Interfaces;

using ClosedXML.Excel;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Infrastructure.Reports;

public class ReportExportService : IReportExportService
{
    private readonly ILogger<ReportExportService> _logger;

    public ReportExportService(ILogger<ReportExportService> logger)
    {
        _logger = logger;
    }

    public async Task<byte[]> ExportToExcelAsync(ReportResponseDto  reportResult)
    {
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("تقرير");
        
        // Arabic support - set right-to-left
        worksheet.RightToLeft = true;
        
        // Add headers with Arabic styling
        for (int i = 0; i < reportResult.Columns.Count; i++)
        {
            var cell = worksheet.Cell(1, i + 1);
            cell.Value = reportResult.Columns[i].Label;
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = 12;
            cell.Style.Fill.BackgroundColor = XLColor.LightBlue;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }
        
        // Add data rows
        for (int row = 0; row < reportResult.Rows.Count; row++)
        {
            var dataRow = reportResult.Rows[row];
            for (int col = 0; col < reportResult.Columns.Count; col++)
            {
                var columnKey = reportResult.Columns[col].Key;
                if (dataRow.ContainsKey(columnKey))
                {
                    var cell = worksheet.Cell(row + 2, col + 1);
                    var value = dataRow[columnKey];
                    
                    // Handle different data types
                    if (value is DateTime dateValue)
                    {
                        cell.Value = dateValue;
                        cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                    }
                    else if (value is decimal || value is double || value is float)
                    {
                        cell.Value = Convert.ToDecimal(value);
                        cell.Style.NumberFormat.Format = "#,##0.00";
                    }
                    else
                    {
                        cell.Value = value?.ToString();
                    }
                    
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                }
            }
        }
        
        // Auto-fit columns
        worksheet.Columns().AdjustToContents();
        
        // Add timestamp and metadata
        worksheet.Cell(reportResult.Rows.Count + 3, 1).Value = "تم التصدير في:";
        worksheet.Cell(reportResult.Rows.Count + 3, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        worksheet.Cell(reportResult.Rows.Count + 3, 2).Style.Font.Italic = true;
        
        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }


    public Task<byte[]> ExportToCsvAsync(ReportResponseDto reportResult)
    {
        var csv = new StringBuilder();
        
        // Headers
        csv.AppendLine(string.Join(",", reportResult.Columns.Select(c => $"\"{c.Label}\"")));
        
        // Data
        foreach (var row in reportResult.Rows)
        {
            var rowValues = reportResult.Columns.Select(col =>
            {
                if (row.TryGetValue(col.Key, out var value))
                    return $"\"{value}\"";
                return "\"\"";
            });
            
            csv.AppendLine(string.Join(",", rowValues));
        }
        
        return Task.FromResult(Encoding.UTF8.GetBytes(csv.ToString()));
    }

    public Task<byte[]> ExportToPdfAsync(ReportResponseDto reportResult)
    {
        // You could use iTextSharp.LGPLv2.Core or QuestPDF for PDF
        throw new NotImplementedException("PDF export not implemented yet");
    }
}