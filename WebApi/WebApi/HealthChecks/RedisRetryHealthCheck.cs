namespace WebApi.HealthChecks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class RedisRetryHealthCheck : IHealthCheck
{
    private const int MaxRetries = 3;
    private const string ProbeKey = "healthcheck:redis:probe";

    private readonly IDistributedCache _cache;

    public RedisRetryHealthCheck(IDistributedCache cache) => _cache = cache;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                await _cache.SetStringAsync(ProbeKey, DateTime.UtcNow.ToString("O"), cancellationToken);
                return HealthCheckResult.Healthy($"Redis responded on attempt {attempt}/{MaxRetries}.");
            }
            catch (Exception) when (attempt < MaxRetries)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt), cancellationToken);
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy($"Redis unreachable after {MaxRetries} attempts.", ex);
            }
        }

        return HealthCheckResult.Unhealthy("Redis unreachable.");
    }
}