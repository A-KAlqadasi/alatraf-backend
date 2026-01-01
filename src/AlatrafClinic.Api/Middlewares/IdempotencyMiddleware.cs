using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Http;

public sealed class IdempotencyMiddleware
{
    private const string HeaderName = "X-Idempotency-Key";
    private readonly RequestDelegate _next;

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IIdempotencyContext idempotencyContext)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var key))
        {
            await _next(context);
            return;
        }

        context.Request.EnableBuffering();

        using var reader = new StreamReader(
            context.Request.Body,
            Encoding.UTF8,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        idempotencyContext.Set(
    key: key.ToString(),
    route: $"{context.Request.Method}:{context.Request.Path}",
    requestHash: Convert.ToHexString(
        SHA256.HashData(Encoding.UTF8.GetBytes(body)))
);

        await _next(context);
    }
}
