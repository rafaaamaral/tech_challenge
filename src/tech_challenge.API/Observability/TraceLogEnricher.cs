using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace tech_challenge.API.Observability;

public sealed class TraceLogEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (Activity.Current is not { } activity) return;

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("trace_id", activity.TraceId.ToHexString()));
        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("span_id", activity.SpanId.ToHexString()));
    }
}
