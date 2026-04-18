namespace ControlAgentNet.Core.Abstractions;

/// <summary>
/// Provides a mechanism to invalidate cached agent instances so that
/// the next invocation picks up the latest configuration.
/// </summary>
public interface IAgentCacheInvalidator
{
    /// <summary>
    /// Removes the cached agent identified by <paramref name="agentId"/> from the
    /// internal cache. The agent will be re-created with the current options on the
    /// next call to <c>RunAsync</c> or <c>StreamAsync</c>.
    /// </summary>
    /// <param name="agentId">The agent identifier (matches <see cref="Core.Models.AgentOptions.Id"/>).</param>
    void InvalidateAgent(string agentId);
}
