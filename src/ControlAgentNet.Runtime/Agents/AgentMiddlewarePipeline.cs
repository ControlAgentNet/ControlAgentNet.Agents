using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ControlAgentNet.Core.Abstractions;
using ControlAgentNet.Core.Models;

namespace ControlAgentNet.Runtime.Agents;

/// <summary>
/// Orchestrates the execution of multiple middlewares before finally calling the agent runner.
/// </summary>
public sealed class AgentMiddlewarePipeline
{
    private readonly IAgentMiddleware[] _reversedMiddlewares;
    private readonly ILogger<AgentMiddlewarePipeline> _logger;

    public AgentMiddlewarePipeline(
        IEnumerable<IAgentMiddleware> middlewares,
        ILogger<AgentMiddlewarePipeline> logger)
    {
        // Cache reversed middlewares array to avoid .Reverse() allocations on every request
        _reversedMiddlewares = middlewares.Reverse().ToArray();
        _logger = logger;
    }

    public async Task<OutgoingMessage> ExecuteAsync(
        IncomingMessage message,
        Func<AgentContext, CancellationToken, Task<OutgoingMessage>> terminal,
        CancellationToken cancellationToken)
    {
        var context = new AgentContext { Message = message };

        // Build the chain from last to first
        AgentDelegate chain = (ctx, ct) => terminal(ctx, ct);

        foreach (var middleware in _reversedMiddlewares)
        {
            var next = chain;
            chain = (ctx, ct) => middleware.InvokeAsync(ctx, next, ct);
        }

        return await chain(context, cancellationToken);
    }
}
