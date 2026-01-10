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

    public async Task<byte[]> ExportToExcelWithBatchAsync(ReportResponseDto reportResult)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("تقرير");
        
        // Arabic support - set right-to-left
        worksheet.RightToLeft = true;
        
        // In ClosedXML, disable calculation for performance
        workbook.CalculateMode = XLCalculateMode.Manual;
        // OR if you want to disable events:
        // workbook.EventTrackingEnabled = false; // Not available in all versions
        
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
        
        // Batch processing for large datasets
        const int batchSize = 1000;
        int totalRows = reportResult.Rows.Count;
        
        for (int batchStart = 0; batchStart < totalRows; batchStart += batchSize)
        {
            int batchEnd = Math.Min(batchStart + batchSize, totalRows);
            
            for (int rowIndex = batchStart; rowIndex < batchEnd; rowIndex++)
            {
                var dataRow = reportResult.Rows[rowIndex];
                int excelRow = rowIndex + 2; // +2 for header row and 1-based index
                
                for (int col = 0; col < reportResult.Columns.Count; col++)
                {
                    var columnKey = reportResult.Columns[col].Key;
                    if (dataRow.ContainsKey(columnKey))
                    {
                        var cell = worksheet.Cell(excelRow, col + 1);
                        var value = dataRow[columnKey];
                        
                        // Handle different data types
                        SetCellValue(cell, value);
                        cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    }
                }
            }
            
            // Optional: Add progress logging for large exports
            if (totalRows > 10000 && batchEnd % 10000 == 0)
            {
                _logger.LogDebug("Excel export progress: {Processed}/{Total} rows", batchEnd, totalRows);
            }
        }
        
        // Auto-fit columns (only for reasonable sizes)
        if (totalRows < 50000)
        {
            worksheet.Columns().AdjustToContents();
        }
        else
        {
            // For very large datasets, set fixed width to improve performance
            worksheet.Columns().Width = 15;
        }
        
        // Add timestamp in Arabic
        var lastRow = totalRows + 3;
        worksheet.Cell(lastRow, 1).Value = "تم التصدير في:";
        worksheet.Cell(lastRow, 2).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        worksheet.Cell(lastRow, 2).Style.Font.Italic = true;
        
        using var memoryStream = new MemoryStream();
        workbook.SaveAs(memoryStream);
        return memoryStream.ToArray();
    }

    private void SetCellValue(IXLCell cell, object? value)
    {
        if (value == null)
        {
            cell.Value = string.Empty;
            return;
        }
        
        switch (value)
        {
            case DateTime dateValue:
                cell.Value = dateValue;
                cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                break;
                
            case DateTimeOffset dateOffset:
                cell.Value = dateOffset.DateTime;
                cell.Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
                break;
                
            case decimal decimalValue:
                cell.Value = decimalValue;
                cell.Style.NumberFormat.Format = "#,##0.00";
                break;
                
            case double doubleValue:
                cell.Value = doubleValue;
                cell.Style.NumberFormat.Format = "#,##0.00";
                break;
                
            case float floatValue:
                cell.Value = floatValue;
                cell.Style.NumberFormat.Format = "#,##0.00";
                break;
                
            case int intValue:
                cell.Value = intValue;
                cell.Style.NumberFormat.Format = "#,##0";
                break;
                
            case long longValue:
                cell.Value = longValue;
                cell.Style.NumberFormat.Format = "#,##0";
                break;
                
            case bool boolValue:
                cell.Value = boolValue ? "نعم" : "لا"; // Arabic Yes/No
                break;
                
            default:
                cell.Value = value?.ToString();
                break;
        }
    }

    public Task<byte[]> ExportToPdfAsync(ReportResponseDto reportResult)
    {
        // You could use iTextSharp.LGPLv2.Core or QuestPDF for PDF
        throw new NotImplementedException("PDF export not implemented yet");
    }

}