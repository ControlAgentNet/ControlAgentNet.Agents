namespace ControlAgentNet.Core.Models;

public sealed record PolicyContext(
    string? TenantId = null,
    string? AgentId = null,
    string? ChannelId = null,
    string? UserId = null);
