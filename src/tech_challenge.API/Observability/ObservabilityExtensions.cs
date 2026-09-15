using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry;
using System.Diagnostics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using Serilog.Templates;
using tech_challenge.Application.Observability;

namespace tech_challenge.API.Observability;

public static class ObservabilityExtensions
{
    public static ExpressionTemplate JsonLogFormatter() => new(
        "{ {timestamp: @t, status: @l, message: @m, exception: @x, ..@p} }\n");

    public static void AddObservability(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var service = config["OTEL_SERVICE_NAME"] ?? "tech-challenge-api";
        var environment = config["DD_ENV"] ?? builder.Environment.EnvironmentName.ToLowerInvariant();
        var version = config["DD_VERSION"] ?? "local";

        builder.Services.AddSerilog((services, logger) => logger
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .ReadFrom.Configuration(config)
            .Enrich.FromLogContext()
            .Enrich.With<TraceLogEnricher>()
            .Enrich.WithProperty("service", service)
            .Enrich.WithProperty("env", environment)
            .Enrich.WithProperty("version", version)
            .WriteTo.Console(JsonLogFormatter()));

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<CorrelationIdHandler>();
        builder.Services.ConfigureHttpClientDefaults(http => http.AddHttpMessageHandler<CorrelationIdHandler>());
        builder.Services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("postgresql", tags: ["ready"], timeout: TimeSpan.FromSeconds(3));

        if (!config.GetValue<bool>("Observability:Enabled")) return;

        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(service, serviceVersion: version, serviceInstanceId: config["POD_UID"] ?? Environment.MachineName)
                .AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment.name"] = environment,
                    ["deployment.environment"] = environment
                }))
            .WithTracing(tracing => tracing
                .AddSource(OrdemServicoTelemetry.SourceName, "Npgsql")
                .AddAspNetCoreInstrumentation(options =>
                {
                    options.Filter = context => !context.Request.Path.StartsWithSegments("/health");
                })
                .AddHttpClientInstrumentation()
                .AddProcessor(new DatabaseTraceProcessor())
                .AddOtlpExporter("traces", _ => { }))
            .WithMetrics(metrics => metrics
                .AddMeter(OrdemServicoTelemetry.SourceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter("metrics", _ => { }));
    }

    public static void MapObservabilityHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions { Predicate = _ => false });
        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = (context, report) => context.Response.WriteAsJsonAsync(new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(entry => new { name = entry.Key, status = entry.Value.Status.ToString() })
            })
        });
    }

    private sealed class DatabaseTraceProcessor : BaseProcessor<Activity>
    {
        public override void OnEnd(Activity activity)
        {
            activity.SetTag("db.query.text", null);
            activity.SetTag("db.statement", null);
        }
    }
}
