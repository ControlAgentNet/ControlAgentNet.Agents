namespace ControlAgentNet.Core.Models;

public sealed record PolicyAuditEntry(
    string ScopeType,
    string ScopeId,
    bool IsEnabled,
    DateTimeOffset ChangedAt,
    string Source,
    string? PolicyValue = null,
    string? TenantId = null,
    string? AgentId = null,
    string? ChannelId = null,
    string? UserId = null,
    string? Actor = null,
    string? Reason = null,
    string? CorrelationId = null,
    DateTimeOffset? UpdatedAt = null);
