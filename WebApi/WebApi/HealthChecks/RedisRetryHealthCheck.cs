namespace WebApi.HealthChecks;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

public class RedisRetryHealthCheck : IHealthCheck
{
    private const int MaxRetries = 3;

    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisRetryHealthCheck(IConnectionMultiplexer connectionMultiplexer) =>
        _connectionMultiplexer = connectionMultiplexer;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; attempt <= MaxRetries; attempt++)
        {
            try
            {
                var db = _connectionMultiplexer.GetDatabase();
                var latency = await db.PingAsync();
                return HealthCheckResult.Healthy($"Redis responded in {latency.TotalMilliseconds}ms (attempt {attempt}/{MaxRetries}).");
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