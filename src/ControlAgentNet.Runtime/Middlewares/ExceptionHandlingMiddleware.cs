using Microsoft.Extensions.Logging;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ControlAgentNet.Runtime.Middlewares;

/// <summary>
/// A pipeline middleware that catches all exceptions and returns a configurable
/// error message to the user. The default message is generic to avoid leaking internals.
/// Override <see cref="AgentOptions.ErrorMessage"/> to customize.
/// </summary>
public sealed class ExceptionHandlingMiddleware : IAgentMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly AgentOptions _agentOptions;

    public ExceptionHandlingMiddleware(
        ILogger<ExceptionHandlingMiddleware> logger,
        Microsoft.Extensions.Options.IOptions<AgentOptions> agentOptions)
    {
        _logger = logger;
        _agentOptions = agentOptions.Value;
    }

    public async Task<OutgoingMessage> InvokeAsync(AgentContext context, AgentDelegate next, CancellationToken cancellationToken)
    {
        try
        {
            return await next(context, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred while processing message {CorrelationId}", context.Message.CorrelationId);

            var baseMessage = string.IsNullOrWhiteSpace(_agentOptions.ErrorMessage)
                ? "An internal error occurred while processing your request. Please try again later."
                : _agentOptions.ErrorMessage;

            var errorMessage = $"{baseMessage} (Correlation ID: {context.Message.CorrelationId})";

            return new OutgoingMessage
            {
                ConversationId = context.Message.ConversationId,
                CorrelationId = context.Message.CorrelationId,
                ChannelId = context.Message.ChannelId,
                ChannelType = context.Message.ChannelType,
                Text = errorMessage,
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }
}
