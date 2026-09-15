using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.Extensions.Logging;
using tech_challenge.Application.Exceptions;
using tech_challenge.Domain.Exceptions;

namespace tech_challenge.Application.Observability;

public static class OrdemServicoTelemetry
{
    public const string SourceName = "TechChallenge.OrdensServico";
    public static readonly ActivitySource ActivitySource = new(SourceName);
    private static readonly Meter Meter = new(SourceName);
    private static readonly Counter<long> Operations = Meter.CreateCounter<long>("techchallenge.os.operations", "{operation}");
    private static readonly Counter<long> Failures = Meter.CreateCounter<long>("techchallenge.os.failures", "{failure}");
    private static readonly Histogram<double> Duration = Meter.CreateHistogram<double>("techchallenge.os.duration", "s");

    public static async Task<T> ExecuteAsync<T>(string operation, Func<Task<T>> action, ILogger logger, int? ordemServicoId = null)
    {
        using var activity = ActivitySource.StartActivity($"os.{operation}");
        activity?.SetTag("os.operation", operation);
        if (ordemServicoId.HasValue) activity?.SetTag("os.id", ordemServicoId.Value);
        using var scope = logger.BeginScope(new Dictionary<string, object?>
        {
            ["os_operation"] = operation,
            ["os_id"] = ordemServicoId
        });
        var started = Stopwatch.GetTimestamp();
        var outcome = "success";
        try
        {
            var result = await action();
            activity?.SetStatus(ActivityStatusCode.Ok);
            logger.LogInformation("Operacao de OS {Operation} concluida", operation);
            return result;
        }
        catch (Exception exception)
        {
            outcome = exception is NotFoundException or DomainException or InvalidOperationException or UnauthorizedAccessException
                ? "rejected" : exception is OperationCanceledException ? "cancelled" : "error";
            activity?.SetStatus(ActivityStatusCode.Error, exception.GetType().Name);
            activity?.SetTag("error.type", exception.GetType().FullName);
            Failures.Add(1, new("operation", operation), new("outcome", outcome));
            if (outcome == "error")
                logger.LogError(exception, "Falha na operacao de OS {Operation}", operation);
            else
                logger.LogWarning("Operacao de OS {Operation} nao concluida: {Outcome} ({ErrorType})", operation, outcome, exception.GetType().Name);
            throw;
        }
        finally
        {
            activity?.SetTag("os.outcome", outcome);
            var tags = new TagList { { "operation", operation }, { "outcome", outcome } };
            Operations.Add(1, tags);
            Duration.Record(Stopwatch.GetElapsedTime(started).TotalSeconds, tags);
        }
    }

    public static Task ExecuteAsync(string operation, Func<Task> action, ILogger logger, int? ordemServicoId = null) =>
        ExecuteAsync(operation, async () => { await action(); return true; }, logger, ordemServicoId);
}
