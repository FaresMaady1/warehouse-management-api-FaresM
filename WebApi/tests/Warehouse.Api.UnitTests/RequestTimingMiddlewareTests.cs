namespace Warehouse.Api.UnitTests;

using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebApi.Middleware;
using Xunit;

public class RequestTimingMiddlewareTests
{
    private class FakeLogger<T> : ILogger<T>
    {
        public List<string> Messages { get; } = new();
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null!;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            Messages.Add(formatter(state, exception));
    }

    // RequestTimingMiddleware
    [Fact]
    public async Task InvokeAsync_SlowRequest_LogsPathStatusCodeAndElapsedMs()
    {
        var logger = new FakeLogger<RequestTimingMiddleware>();
        var middleware = new RequestTimingMiddleware(async ctx =>
        {
            await Task.Delay(600);
            ctx.Response.StatusCode = 200;
        }, logger);

        var context = new DefaultHttpContext();
        context.Request.Method = "GET";
        context.Request.Path = "/api/products";

        await middleware.InvokeAsync(context);

        logger.Messages.Should().ContainSingle(m => m.Contains("/api/products") && m.Contains("200"));
    }

    // RequestTimingMiddleware
    [Fact]
    public async Task InvokeAsync_FastRequest_DoesNotLog()
    {
        var logger = new FakeLogger<RequestTimingMiddleware>();
        var middleware = new RequestTimingMiddleware(ctx =>
        {
            ctx.Response.StatusCode = 200;
            return Task.CompletedTask;
        }, logger);

        await middleware.InvokeAsync(new DefaultHttpContext());

        logger.Messages.Should().BeEmpty();
    }
}