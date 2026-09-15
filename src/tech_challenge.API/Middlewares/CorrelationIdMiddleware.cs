using System.Diagnostics;
using Serilog.Context;

namespace tech_challenge.API.Middlewares;

public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    public const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var values = context.Request.Headers[HeaderName];
        var candidate = values.Count == 1 ? values[0] : null;
        var correlationId = IsValid(candidate) ? candidate! : Guid.NewGuid().ToString("N");
        context.TraceIdentifier = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });
        Activity.Current?.SetTag("correlation.id", correlationId);

        using (LogContext.PushProperty("correlation_id", correlationId))
        {
            await next(context);
        }
    }

    public static bool IsValid(string? value) => value is { Length: > 0 and <= 128 }
        && value.All(c => char.IsAsciiLetterOrDigit(c) || c is '-' or '_' or '.');
}
