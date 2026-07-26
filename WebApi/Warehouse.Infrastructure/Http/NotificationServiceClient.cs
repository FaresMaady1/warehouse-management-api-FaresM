namespace Warehouse.Infrastructure.Http;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Warehouse.Domain.Notifications;

public class NotificationServiceClient : INotificationServiceClient
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationServiceClient> _logger;

    public NotificationServiceClient(HttpClient httpClient, ILogger<NotificationServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<int?> GetUnreadCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<UnreadCountResponse>(
                "api/notifications/unread-count", JsonOptions, cancellationToken);

            return result?.Count;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // Timeout or connection refused - the Notification Service being down shouldn't
            // break the warehouse API's own endpoints.
            _logger.LogWarning(ex, "Notification Service unread-count call failed or timed out.");
            return null;
        }
    }

    private record UnreadCountResponse(int Count);
}
