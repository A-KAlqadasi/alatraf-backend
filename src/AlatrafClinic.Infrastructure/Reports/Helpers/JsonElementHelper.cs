using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;

public static class JsonElementHelper
{
    private static readonly string[] _dateFormats = new[]
    {
        "yyyy-MM-dd",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-dd HH:mm:ss",
        "dd/MM/yyyy",
        "MM/dd/yyyy",
        "yyyy/MM/dd"
    };

    public static object ExtractValue(JsonElement element, string? dataTypeHint = null)
    {
        try
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => ExtractStringValue(element.GetString()!, dataTypeHint),
                JsonValueKind.Number => ExtractNumber(element),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => DBNull.Value,
                JsonValueKind.Undefined => DBNull.Value,
                _ => element.GetRawText() // Fallback for arrays/objects
            };
        }
        catch (Exception ex)
        {
            throw new ValidationException($"Invalid filter value: {ex.Message}", ex);
        }
    }

    private static object ExtractStringValue(string value, string? dataTypeHint)
    {
        // If dataTypeHint suggests it's a date/time column, try to parse as date
        if (!string.IsNullOrEmpty(dataTypeHint) && 
            (dataTypeHint.Contains("date", StringComparison.OrdinalIgnoreCase) || 
             dataTypeHint.Contains("time", StringComparison.OrdinalIgnoreCase)))
        {
            // Try to parse as DateTime
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateTime))
            {
                // If it's datetimeoffset in SQL, convert to DateTimeOffset
                if (dataTypeHint.Contains("datetimeoffset", StringComparison.OrdinalIgnoreCase))
                {
                    return new DateTimeOffset(dateTime);
                }
                return dateTime;
            }
            
            // Try specific formats
            foreach (var format in _dateFormats)
            {
                if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
                {
                    if (dataTypeHint.Contains("datetimeoffset", StringComparison.OrdinalIgnoreCase))
                    {
                        return new DateTimeOffset(dateTime);
                    }
                    return dateTime;
                }
            }
        }

        // If not a date or can't parse, return as string
        return value;
    }

    private static object ExtractNumber(JsonElement element)
    {
        if (element.TryGetInt32(out var intValue)) return intValue;
        if (element.TryGetInt64(out var longValue)) return longValue;
        if (element.TryGetDecimal(out var decimalValue)) return decimalValue;
        if (element.TryGetDouble(out var doubleValue)) return doubleValue;
        
        return element.GetRawText();
    }

    public static object? ExtractNullableValue(JsonElement? element, string? dataTypeHint = null)
    {
        if (!element.HasValue || element.Value.ValueKind == JsonValueKind.Null)
            return DBNull.Value;
            
        return ExtractValue(element.Value, dataTypeHint);
    }
}