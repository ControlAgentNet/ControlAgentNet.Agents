using Microsoft.Extensions.Logging;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;

namespace ControlAgentNet.Runtime.Agents;

public sealed class ControlAgentOrchestrator : IAgentOrchestrator
{
    private readonly IAgentEngine _engine;
    private readonly AgentMiddlewarePipeline _pipeline;
    private readonly IAgentContextProvider _contextProvider;
    private readonly ILogger<ControlAgentOrchestrator> _logger;

    public ControlAgentOrchestrator(
        IAgentEngine engine,
        AgentMiddlewarePipeline pipeline,
        IAgentContextProvider contextProvider,
        ILogger<ControlAgentOrchestrator> logger)
    {
        _engine = engine;
        _pipeline = pipeline;
        _contextProvider = contextProvider;
        _logger = logger;
    }

    public async Task<OutgoingMessage> ProcessAsync(IncomingMessage message, CancellationToken cancellationToken = default)
    {
        using var logScope = _logger.BeginScope(new System.Collections.Generic.Dictionary<string, object>
        {
            ["CorrelationId"] = message.CorrelationId ?? "N/A",
            ["ConversationId"] = message.ConversationId,
            ["UserId"] = message.UserId
        });

        _logger.LogTrace("Orchestrating message through the middleware pipeline");

        return await _pipeline.ExecuteAsync(message, async (ctx, ct) =>
        {
            var previousContext = _contextProvider.Current;
            _contextProvider.Current = ctx;

            try
            {
                var msg = ctx.Message;
                _logger.LogInformation("Processing {ChannelId} message from user {UserId}",
                    msg.ChannelId, msg.UserId);

                var result = await _engine.RunAsync(ctx, ct);
                
                if (ctx.Usage == null && result.Usage != null)
                {
                    ctx.Usage = result.Usage;
                }

                return new OutgoingMessage
                {
                    ConversationId = msg.ConversationId,
                    CorrelationId = msg.CorrelationId,
                    ChannelId = msg.ChannelId,
                    ChannelType = msg.ChannelType,
                    Text = !string.IsNullOrWhiteSpace(result.Text) ? result.Text : "I was unable to generate a response.",
                    Thoughts = ctx.Thoughts.ToList(),
                    Usage = ctx.Usage,
                    Timestamp = DateTimeOffset.UtcNow
                };
            }
            finally
            {
                _contextProvider.Current = previousContext;
            }
        }, cancellationToken);
    }

    public async Task<OutgoingMessage> ProcessStreamAsync(IncomingMessage message, CancellationToken cancellationToken = default)
    {
        using var logScope = _logger.BeginScope(new System.Collections.Generic.Dictionary<string, object>
        {
            ["CorrelationId"] = message.CorrelationId ?? "N/A",
            ["ConversationId"] = message.ConversationId,
            ["UserId"] = message.UserId
        });

        _logger.LogTrace("Orchestrating streaming message through the middleware pipeline");

        return await _pipeline.ExecuteAsync(message, (ctx, ct) =>
        {
            var previousContext = _contextProvider.Current;
            _contextProvider.Current = ctx;

            try
            {
                var msg = ctx.Message;
                _logger.LogInformation("Processing {ChannelId} stream from user {UserId}",
                    msg.ChannelId, msg.UserId);

                var stream = _engine.StreamAsync(ctx, ct);

                var response = new OutgoingMessage
                {
                    ConversationId = msg.ConversationId,
                    CorrelationId = msg.CorrelationId,
                    ChannelId = msg.ChannelId,
                    ChannelType = msg.ChannelType,
                    Text = string.Empty,
                    ContentStream = stream,
                    Thoughts = ctx.Thoughts.ToList(),
                    Usage = ctx.Usage,
                    Timestamp = DateTimeOffset.UtcNow
                };

                return Task.FromResult(response);
            }
            finally
            {
                _contextProvider.Current = previousContext;
            }
        }, cancellationToken);
    }
}
