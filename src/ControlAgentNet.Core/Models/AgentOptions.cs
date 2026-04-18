namespace ControlAgentNet.Core.Models;

public sealed class AgentOptions
{
    public const string SectionName = "Agent";

    public string Id { get; set; } = "controlagentnet-agent";

    public string? TenantId { get; set; }

    public string Name { get; set; } = "ControlAgentNet";

    public string Description { get; set; } = "MAF-based modular agent";

    public string Instructions { get; set; } = "You are ControlAgentNet, a helpful and direct agent.";

    /// <summary>
    /// Custom error message returned to the user when an unhandled exception occurs.
    /// If empty, a generic English message is used.
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// Sliding-expiration TTL for the cached <c>AIHostAgent</c> instance.
    /// When the agent has not been used for this duration, it is evicted and
    /// re-created with the latest configuration on the next invocation.
    /// Defaults to 30 minutes. Set to <see cref="TimeSpan.Zero"/> to disable
    /// TTL-based expiration (the agent is cached indefinitely until an explicit
    /// invalidation or an options-change event).
    /// </summary>
    public TimeSpan CacheTtl { get; set; } = TimeSpan.FromMinutes(30);

}
