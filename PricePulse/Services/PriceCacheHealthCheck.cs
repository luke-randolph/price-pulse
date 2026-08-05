using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PricePulse.Services;

// Reports whether the price cache has data. Deliberately never returns Unhealthy: the app is
// designed to serve (with a loading state) through a FRED outage, and a 503 here would tell the
// host to recycle a container that is working exactly as intended. Degraded still returns 200.
public class PriceCacheHealthCheck : IHealthCheck
{
    private readonly PriceStore _store;

    public PriceCacheHealthCheck(PriceStore store)
    {
        _store = store;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_store.IsWarm
            ? HealthCheckResult.Healthy("Price cache is warm.")
            : HealthCheckResult.Degraded("Price cache is empty; awaiting a successful FRED load."));
    }
}
