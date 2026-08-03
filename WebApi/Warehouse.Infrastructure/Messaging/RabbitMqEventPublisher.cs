namespace Warehouse.Infrastructure.Messaging;

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Warehouse.Domain.Events;

public class RabbitMqEventPublisher : IEventPublisher, IDisposable
{
    private readonly string _exchange;
    private readonly ILogger<RabbitMqEventPublisher> _logger;
    private readonly IConnection? _connection;
    private readonly IModel? _channel;

    public RabbitMqEventPublisher(
        string hostName, int port, string userName, string password, string exchange,
        ILogger<RabbitMqEventPublisher> logger)
    {
        _exchange = exchange;
        _logger = logger;

        try
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(_exchange, ExchangeType.Topic, durable: true);
        }
        catch (Exception ex)
        {
            // RabbitMQ might not be running yet (or at all, for students not testing this part).
            // We don't want that to take down the whole warehouse API - PublishAsync just
            // no-ops below when _channel is null.
            _logger.LogWarning(ex, "Could not connect to RabbitMQ at startup. Events won't be published until it's reachable.");
        }
    }

    public Task PublishAsync<T>(string routingKey, T message, CancellationToken cancellationToken = default) where T : class
    {
        if (_channel == null)
        {
            _logger.LogWarning("Skipped publishing {RoutingKey} - RabbitMQ channel is not available.", routingKey);
            return Task.CompletedTask;
        }

        try
        {
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";

            _channel.BasicPublish(_exchange, routingKey, properties, body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event with routing key {RoutingKey}.", routingKey);
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
