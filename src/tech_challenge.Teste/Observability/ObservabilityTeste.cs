using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using tech_challenge.API.Middlewares;
using tech_challenge.API.Observability;
using tech_challenge.Application.Exceptions;
using tech_challenge.Application.Common.Interfaces;
using tech_challenge.Application.Observability;
using tech_challenge.Infrastructure.Persistence.Context;

namespace tech_challenge.Teste.Observability;

[CollectionDefinition("Observability", DisableParallelization = true)]
public class ObservabilityCollection;

[Collection("Observability")]
public class ObservabilityTeste
{
    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData("request-123_abc.1", true)]
    [InlineData("request 123", false)]
    [InlineData("bad\r\nheader", false)]
    public void CorrelationId_ValidaFormato(string? value, bool valid)
    {
        Assert.Equal(valid, CorrelationIdMiddleware.IsValid(value));
        Assert.False(CorrelationIdMiddleware.IsValid(new string('a', 129)));
    }

    [Theory]
    [InlineData("/ok", 200)]
    [InlineData("/rejected", 400)]
    [InlineData("/missing", 404)]
    [InlineData("/fail", 500)]
    [InlineData("/denied", 401)]
    public async Task CorrelationId_PreservadoInclusiveEmErros(string path, int status)
    {
        await using var app = await CreateAppAsync();
        using var client = app.GetTestClient();
        client.DefaultRequestHeaders.Add(CorrelationIdMiddleware.HeaderName, "tc3-test-123");
        using var response = await client.GetAsync(path);
        Assert.Equal(status, (int)response.StatusCode);
        Assert.Equal("tc3-test-123", response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single());
        if (status == 500) Assert.DoesNotContain("secret-detail", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("bad value")]
    [InlineData("one,two")]
    public async Task CorrelationId_GeraIdentificadorQuandoAusenteOuInvalido(string? value)
    {
        await using var app = await CreateAppAsync();
        using var client = app.GetTestClient();
        if (value != null) client.DefaultRequestHeaders.TryAddWithoutValidation(CorrelationIdMiddleware.HeaderName, value);
        using var response = await client.GetAsync("/ok");
        Assert.True(Guid.TryParseExact(response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).Single(), "N", out _));
    }

    [Fact]
    public async Task Logs_JsonComTraceEIsolamentoDeCorrelationId()
    {
        using var writer = new StringWriter();
        using var logger = new LoggerConfiguration().Enrich.FromLogContext().Enrich.With<TraceLogEnricher>()
            .WriteTo.Sink(new JsonSink(writer)).CreateLogger();
        using var parent = new Activity("request").SetIdFormat(ActivityIdFormat.W3C).Start();
        var middleware = new CorrelationIdMiddleware(async context =>
        {
            await Task.Yield();
            logger.Information("Teste {RequestId}", context.TraceIdentifier);
        });
        var contexts = Enumerable.Range(0, 10).Select(index =>
        {
            var context = new DefaultHttpContext();
            context.Request.Headers[CorrelationIdMiddleware.HeaderName] = $"request-{index}";
            return context;
        });
        await Task.WhenAll(contexts.Select(middleware.InvokeAsync));
        logger.Information("Fora da requisicao");
        var lines = writer.ToString().Split('\n', StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(11, lines.Length);
        foreach (var line in lines.Take(10))
        {
            using var document = JsonDocument.Parse(line);
            var json = document.RootElement;
            Assert.Equal(json.GetProperty("RequestId").GetString(), json.GetProperty("correlation_id").GetString());
            Assert.Equal(parent.TraceId.ToHexString(), json.GetProperty("trace_id").GetString());
            Assert.Equal(parent.SpanId.ToHexString(), json.GetProperty("span_id").GetString());
            Assert.Equal("Information", json.GetProperty("status").GetString());
            Assert.True(json.TryGetProperty("timestamp", out _));
            Assert.True(json.TryGetProperty("message", out _));
        }
        using var outside = JsonDocument.Parse(lines.Last());
        Assert.False(outside.RootElement.TryGetProperty("correlation_id", out _));
    }

    [Fact]
    public async Task CorrelationId_PropagaEmHttpClient()
    {
        var context = new DefaultHttpContext { TraceIdentifier = "outgoing-test" };
        var receiver = new CaptureHandler();
        using var handler = new CorrelationIdHandler(new HttpContextAccessor { HttpContext = context }) { InnerHandler = receiver };
        using var client = new HttpClient(handler);
        using var response = await client.GetAsync("http://example.test/");
        Assert.Equal("outgoing-test", receiver.CorrelationId);
    }

    [Theory]
    [InlineData("success")]
    [InlineData("rejected")]
    [InlineData("error")]
    [InlineData("cancelled")]
    public async Task Operacao_RegistraMetricasETracePreservandoResultado(string outcome)
    {
        var measurements = new List<(string Name, double Value, Dictionary<string, object?> Tags)>();
        using var meterListener = new MeterListener();
        meterListener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Name == OrdemServicoTelemetry.SourceName) listener.EnableMeasurementEvents(instrument);
        };
        meterListener.SetMeasurementEventCallback<long>((instrument, value, tags, _) =>
            measurements.Add((instrument.Name, value, tags.ToArray().ToDictionary(tag => tag.Key, tag => tag.Value))));
        meterListener.SetMeasurementEventCallback<double>((instrument, value, tags, _) =>
            measurements.Add((instrument.Name, value, tags.ToArray().ToDictionary(tag => tag.Key, tag => tag.Value))));
        meterListener.Start();
        Activity? stopped = null;
        using var activityListener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == OrdemServicoTelemetry.SourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity => stopped = activity
        };
        ActivitySource.AddActivityListener(activityListener);
        using var parent = new Activity("request").Start();
        Exception? failure = outcome switch
        {
            "rejected" => new NotFoundException("OS", 42),
            "error" => new IOException("storage failed"),
            "cancelled" => new OperationCanceledException(),
            _ => null
        };
        var task = OrdemServicoTelemetry.ExecuteAsync("entregar", () => failure == null
            ? Task.FromResult(42) : Task.FromException<int>(failure), NullLogger.Instance, 42);
        if (failure == null) Assert.Equal(42, await task);
        else Assert.Same(failure, await Record.ExceptionAsync(async () => await task));

        Assert.NotNull(stopped);
        Assert.Equal(parent.TraceId, stopped.TraceId);
        Assert.Equal(outcome == "success" ? ActivityStatusCode.Ok : ActivityStatusCode.Error, stopped.Status);
        Assert.Equal(outcome, stopped.GetTagItem("os.outcome"));
        Assert.Contains(measurements, m => m.Name == "techchallenge.os.operations" && m.Value == 1 && Equals(m.Tags["outcome"], outcome));
        Assert.Contains(measurements, m => m.Name == "techchallenge.os.duration" && m.Value >= 0);
        Assert.Equal(outcome == "success" ? 0 : 1, measurements.Count(m => m.Name == "techchallenge.os.failures"));
        Assert.All(measurements, m => Assert.Equal(new[] { "operation", "outcome" }, m.Tags.Keys.Order().ToArray()));
    }

    [Fact]
    public async Task Health_BancoIndisponivelAfetaReadinessMasNaoLiveness()
    {
        await using var app = await CreateAppAsync();
        using var client = app.GetTestClient();
        using var live = await client.GetAsync("/health/live");
        using var ready = await client.GetAsync("/health/ready");
        Assert.Equal(HttpStatusCode.OK, live.StatusCode);
        Assert.Equal(HttpStatusCode.ServiceUnavailable, ready.StatusCode);
        var body = await ready.Content.ReadAsStringAsync();
        Assert.Contains("Unhealthy", body);
        Assert.DoesNotContain("Password", body);
    }

    [Fact]
    public async Task OpenTelemetry_ExportaTracesEMetricasPorOtlp()
    {
        var receiver = new CaptureHandler();
        await using var app = await CreateAppAsync(receiver);
        using var client = app.GetTestClient();
        using var response = await client.GetAsync("/operation");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(app.Services.GetRequiredService<TracerProvider>().ForceFlush());
        Assert.True(app.Services.GetRequiredService<MeterProvider>().ForceFlush());
        Assert.Contains(receiver.Exports, export => export.Path == "/v1/traces" && export.Size > 0);
        Assert.Contains(receiver.Exports, export => export.Path == "/v1/metrics" && export.Size > 0);
    }

    private static async Task<WebApplication> CreateAppAsync(CaptureHandler? collector = null)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions { EnvironmentName = "Testing" });
        builder.WebHost.UseTestServer();
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Observability:Enabled"] = (collector != null).ToString(),
            ["OTEL_SERVICE_NAME"] = "tc3-tests"
        });
        builder.AddObservability();
        builder.Services.AddScoped(_ => Mock.Of<IUsuarioLogadoService>());
        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(
            "Host=127.0.0.1;Port=1;Database=unavailable;Username=test;Password=test;Timeout=1"));
        if (collector != null)
        {
            foreach (var signal in new[] { "traces", "metrics" })
            {
                builder.Services.Configure<OtlpExporterOptions>(signal, options =>
                {
                    options.Endpoint = new Uri($"http://collector.test/v1/{signal}");
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    options.HttpClientFactory = () => new HttpClient(collector, disposeHandler: false);
                });
            }
        }
        var app = builder.Build();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<ExceptionMiddleware>();
        app.MapGet("/ok", () => Results.Ok());
        app.MapGet("/denied", () => Results.Unauthorized());
        app.MapGet("/rejected", (Func<IResult>)(() => throw new InvalidOperationException("invalid operation")));
        app.MapGet("/fail", (Func<IResult>)(() => throw new IOException("secret-detail")));
        app.MapGet("/operation", () => OrdemServicoTelemetry.ExecuteAsync("listar", () => Task.FromResult(1), NullLogger.Instance));
        app.MapObservabilityHealthChecks();
        await app.StartAsync();
        return app;
    }

    private sealed class JsonSink(TextWriter writer) : ILogEventSink
    {
        public void Emit(LogEvent logEvent)
        {
            lock (writer) ObservabilityExtensions.JsonLogFormatter().Format(logEvent, writer);
        }
    }

    private sealed class CaptureHandler : HttpMessageHandler
    {
        public string? CorrelationId { get; private set; }
        public ConcurrentBag<(string Path, int Size)> Exports { get; } = [];

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken) =>
            SendAsync(request, cancellationToken).GetAwaiter().GetResult();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.TryGetValues(CorrelationIdMiddleware.HeaderName, out var values)) CorrelationId = values.Single();
            if (request.Content != null)
                Exports.Add((request.RequestUri!.AbsolutePath, (await request.Content.ReadAsByteArrayAsync(cancellationToken)).Length));
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent([]) };
        }
    }
}
