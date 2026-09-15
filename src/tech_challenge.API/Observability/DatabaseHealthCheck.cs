using Microsoft.Extensions.Diagnostics.HealthChecks;
using tech_challenge.Infrastructure.Persistence.Context;

namespace tech_challenge.API.Observability;

public sealed class DatabaseHealthCheck(AppDbContext context) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext healthContext, CancellationToken cancellationToken = default)
    {
        return await context.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("PostgreSQL unavailable");
    }
}
