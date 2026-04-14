using System.Threading;
using System.Threading.Tasks;
using ControlAgentNet.Core.Models;

namespace ControlAgentNet.Core.Abstractions;

/// <summary>
/// Represents the cognitive execution of an AI interaction.
/// </summary>
public interface IAgentEngine
{
    /// <summary>
    /// Executes the AI engine to generate a full response.
    /// </summary>
    /// <summary>
    /// Executes the AI engine to generate a full response, returning rich metadata
    /// such as ModelId, TokenUsage, and FinishReason.
    /// </summary>
    Task<AgentEngineResult> RunAsync(AgentContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Executes the AI engine returning a stream of chunks for real-time interaction.
    /// </summary>
    IAsyncEnumerable<string> StreamAsync(AgentContext context, CancellationToken cancellationToken);
}
