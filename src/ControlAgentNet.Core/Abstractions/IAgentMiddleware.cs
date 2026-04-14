using System.Threading;
using System.Threading.Tasks;
using ControlAgentNet.Core.Models;

namespace ControlAgentNet.Core.Abstractions;

/// <summary>
/// A delegate representing the next step in the agent processing pipeline.
/// </summary>
public delegate Task<OutgoingMessage> AgentDelegate(AgentContext context, CancellationToken cancellationToken);

/// <summary>
/// Represents a middleware that can intercept and modify agent requests and responses.
/// </summary>
public interface IAgentMiddleware
{
    Task<OutgoingMessage> InvokeAsync(AgentContext context, AgentDelegate next, CancellationToken cancellationToken);
}
