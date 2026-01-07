using System.Text.Json;

public static class JsonElementHelper
{
    public static object ExtractValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString()!,
            JsonValueKind.Number => element.TryGetInt64(out var l)
                                        ? l
                                        : element.GetDecimal(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => DBNull.Value,
            _ => throw new NotSupportedException(
                $"Unsupported JSON value kind: {element.ValueKind}")
        };
    }
}
