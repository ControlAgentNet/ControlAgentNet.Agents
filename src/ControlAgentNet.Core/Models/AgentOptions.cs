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

}
