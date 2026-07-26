namespace Warehouse.Notifications.Infrastructure.Messaging;

using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Warehouse.Notifications.Application.Commands.CreateFileUploadedNotification;
using Warehouse.Notifications.Application.Commands.CreateStockLowNotification;
using Warehouse.Notifications.Application.Events;

public class WarehouseEventConsumer : BackgroundService
{
    private const int MaxRetries = 3;
    private const string RetryCountHeader = "x-retry-count";

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WarehouseEventConsumer> _logger;

    private IConnection? _connection;
    private IModel? _channel;

    public WarehouseEventConsumer(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<WarehouseEventConsumer> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested && _channel == null)
        {
            try
            {
                Connect();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "RabbitMQ isn't reachable yet, retrying in 5s...");
                _channel = null;
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        if (_channel == null) return;

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, ea) => await OnMessageReceived(ea);

        _channel.BasicConsume(_configuration["RabbitMq:Queue"]!, autoAck: false, consumer);

        _logger.LogInformation("Notification Service is listening for warehouse events.");

        // BackgroundService needs ExecuteAsync to stay alive for as long as we want to keep consuming.
        while (!stoppingToken.IsCancellationRequested)
            await Task.Delay(1000, stoppingToken);
    }

    private void Connect()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMq:HostName"]!,
            Port = int.Parse(_configuration["RabbitMq:Port"]!),
            UserName = _configuration["RabbitMq:UserName"]!,
            Password = _configuration["RabbitMq:Password"]!
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        var exchange = _configuration["RabbitMq:Exchange"]!;
        var queue = _configuration["RabbitMq:Queue"]!;
        var deadLetterExchange = _configuration["RabbitMq:DeadLetterExchange"]!;
        var deadLetterQueue = _configuration["RabbitMq:DeadLetterQueue"]!;

        _channel.ExchangeDeclare(exchange, ExchangeType.Topic, durable: true);

        // bonnus 3 dead letter 
        _channel.ExchangeDeclare(deadLetterExchange, ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare(deadLetterQueue, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(deadLetterQueue, deadLetterExchange, routingKey: "");

        var queueArgs = new Dictionary<string, object> { { "x-dead-letter-exchange", deadLetterExchange } };
        _channel.QueueDeclare(queue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);

        _channel.QueueBind(queue, exchange, routingKey: "stock.low");
        _channel.QueueBind(queue, exchange, routingKey: "file.uploaded");

        _channel.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);
    }

    private async Task OnMessageReceived(BasicDeliverEventArgs ea)
    {
        var routingKey = ea.RoutingKey;
        var body = Encoding.UTF8.GetString(ea.Body.ToArray());

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            switch (routingKey)
            {
                case "stock.low":
                    var stockLowEvent = JsonSerializer.Deserialize<StockLowDetectedEvent>(body);
                    if (stockLowEvent != null)
                        await mediator.Send(new CreateStockLowNotificationCommand(stockLowEvent));
                    break;

                case "file.uploaded":
                    var fileUploadedEvent = JsonSerializer.Deserialize<WarehouseFileUploadedEvent>(body);
                    if (fileUploadedEvent != null)
                        await mediator.Send(new CreateFileUploadedNotificationCommand(fileUploadedEvent));
                    break;

                default:
                    _logger.LogWarning("Ignoring message with unrecognized routing key {RoutingKey}.", routingKey);
                    break;
            }

            _channel!.BasicAck(ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process a {RoutingKey} message.", routingKey);
            RetryOrDeadLetter(ea);
        }
    }

    private void RetryOrDeadLetter(BasicDeliverEventArgs ea)
    {
        var retryCount = GetRetryCount(ea.BasicProperties);

        if (retryCount < MaxRetries)
        {
            var properties = _channel!.CreateBasicProperties();
            properties.Persistent = true;
            properties.ContentType = "application/json";
            properties.Headers = new Dictionary<string, object> { { RetryCountHeader, retryCount + 1 } };

            _channel.BasicPublish(ea.Exchange, ea.RoutingKey, properties, ea.Body);
            _channel.BasicAck(ea.DeliveryTag, multiple: false);

            _logger.LogWarning("Requeued message for retry {RetryCount}/{MaxRetries}.", retryCount + 1, MaxRetries);
        }
        else
        {
            _logger.LogWarning("Message failed {MaxRetries} times, sending to the dead-letter queue.", MaxRetries);
            _channel!.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
        }
    }

    private static int GetRetryCount(IBasicProperties? properties)
    {
        if (properties?.Headers != null && properties.Headers.TryGetValue(RetryCountHeader, out var value))
            return Convert.ToInt32(value);

        return 0;
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
